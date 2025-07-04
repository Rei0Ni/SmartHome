﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using SmartHome.Enum;
using SmartHome.Application.Exceptions;
using SmartHome.Application.Interfaces.Jwt;
using SmartHome.Application.Interfaces.User;
using SmartHome.Domain.Entities;
using SmartHome.Dto;
using SmartHome.Dto.User;
using System.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using SmartHome.Application.Interfaces.UserAreas;
using SmartHome.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Serilog;
using Microsoft.AspNetCore.Hosting;
using Log = Serilog.Log;

namespace SmartHome.Application.Services.User
{
    public class UserService : IUserService
    {
        private readonly IValidator<LoginDto> _loginValidator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ITotpService _totpService;
        private readonly IMapper _mapper;
        private readonly IUserAreasRepository _userAreasRepository;
        private readonly string ProfilePicturesPath = String.Empty;
        private readonly IWebHostEnvironment _env;

        public UserService(
            IValidator<LoginDto> loginValidator,
            UserManager<ApplicationUser> userManager,
            IJwtTokenService jwtTokenService,
            IMapper mapper,
            IUserAreasRepository userAreasRepository,
            IUserRepository userRepository,
            ITotpService totpService,
            IWebHostEnvironment env)
        {
            _loginValidator = loginValidator;
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _mapper = mapper;
            _userAreasRepository = userAreasRepository;
            _userRepository = userRepository;
            _totpService = totpService;
            _env = env;
            ProfilePicturesPath = $"{_env.WebRootPath}/ProfilePictures/";
            
        }

        public async Task<ApiResponse<object>> CreateAdminUserAsync(RegisterAdminUserDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Username,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return new ApiResponse<object>
                {
                    Status = ApiResponseStatus.Error.ToString(),
                    Message = "User creation failed",
                    Data = result.Errors.Select(e => e.Description)
                };
            }

            await _userManager.AddToRoleAsync(user, System.Enum.GetName(typeof(Role), Role.Admin)!);

            // Generate and store the secret
            string secretKey = _totpService.GenerateSecretKey();
            user.TOTPSecret = secretKey;
            await _userManager.UpdateAsync(user);


            var TotpQRUri = _totpService.GenerateQrCodeUrl(user.UserName, user.TOTPSecret);

            return new ApiResponse<object>
            {
                Status = ApiResponseStatus.Success.ToString(),
                Message = "User created successfully",
                Data = new { UserId = user.Id, TotpQRUri, SecretKey = secretKey}
            };
        }

        public async Task<ApiResponse<object>> CreateNormalUserAsync(RegisterUserDto dto)
        {
            return await CreateUserWithAreasAsync(dto, Role.Normal_User);
        }

        public async Task<ApiResponse<object>> CreateGuestUserAsync(RegisterUserDto dto)
        {
            return await CreateUserWithAreasAsync(dto, Role.Guest);
        }

        public async Task<ApiResponse<object>> CreateUserAsync(RegisterUserDto dto, Role role)
        {
            return await CreateUserWithAreasAsync(dto, role);
        }

        private async Task<ApiResponse<object>> CreateUserWithAreasAsync(RegisterUserDto dto, Role role)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Username,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = string.Empty,
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return new ApiResponse<object>
                {
                    Status = ApiResponseStatus.Error.ToString(),
                    Message = "User creation failed",
                    Data = result.Errors.Select(e => e.Description)
                };
            }

            await _userManager.AddToRoleAsync(user, System.Enum.GetName(typeof(Role), role)!);

            if (dto.AllowedAreas != null && dto.AllowedAreas.Any())
            {
                var filter = Builders<ApplicationUser>.Filter.Eq("UserName", user.UserName);

                if (user != null)
                {
                    var userAreas = new UserAreas
                    {
                        UserId = user.Id,
                        AllowedAreaIds = dto.AllowedAreas
                    };

                    await _userAreasRepository.AddUserAreasAsync(userAreas);
                }

            }

            // Generate and store the secret
            string secretKey = _totpService.GenerateSecretKey();
            user.TOTPSecret = secretKey;
            await _userManager.UpdateAsync(user);


            var TotpQRUri = _totpService.GenerateQrCodeUrl(user.UserName, user.TOTPSecret);

            return new ApiResponse<object>
            {
                Status = ApiResponseStatus.Success.ToString(),
                Message = "User created successfully",
                Data = new { UserId = user.Id, TotpQRUri, SecretKey = secretKey }
            };
        }

        public async Task<ApiResponse<object>> DeleteUserAsync(Guid userId)
        {
            // Find the user by ID
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return new ApiResponse<object>
                {
                    Status = ApiResponseStatus.Error.ToString(),
                    Message = "User not found"
                };
            }

            await _userAreasRepository.DeleteUserAreasAsync(user.Id);

            // Attempt to delete the user
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return new ApiResponse<object>
                {
                    Status = ApiResponseStatus.Error.ToString(),
                    Message = "User deletion failed",
                    Data = result.Errors.Select(e => e.Description)
                };
            }

            return new ApiResponse<object>
            {
                Status = ApiResponseStatus.Success.ToString(),
                Message = "User deleted successfully"
            };
        }

        public async Task<UserAuthenticationState> GetUserAuthenticationStateAsync(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var applicationUser = _mapper.Map<UserAuthenticationState>(user);
                    return applicationUser;
                }
            }

            return new();
        }

        public async Task<ApiResponse<object>> LoginAsync(LoginDto dto)
        {
            ValidationResult result = await _loginValidator.ValidateAsync(dto);

            if (!result.IsValid)
            {
                throw new FluentValidationException(ApiResponseStatus.Error.ToString(), "Invalid Credentials", result.Errors);
            }

            var user = await _userManager.FindByNameAsync(dto.Username);

            if (user == null)
            {
                throw new LoginFailedException(ApiResponseStatus.Error.ToString(), "Invalid Username or Password");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!passwordValid)
            {
                throw new LoginFailedException(ApiResponseStatus.Error.ToString(), "Invalid Username or Password");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var token = _jwtTokenService.GenerateJwtToken(user.Id.ToString(), user.UserName!, user.Email!, user.ProfilePictureUrl!, userRoles);

            user.LastLogin = DateTime.UtcNow;

            var lastLoginUpdated = await _userRepository.UpdateLastLoginValue(user);

            var response = new ApiResponse<object>
            {
                Status = ApiResponseStatus.Success.ToString(),
                Message = lastLoginUpdated ? "Login Successful" : "Login Successful But Couldn't Update Login Time",
                Data = new
                {
                    Token = token
                }
            };
            return response;
        }

        public async Task<ApiResponse<object>> UpdateAdminUserProfileAsync(UpdateAdminUserProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.Id);
            if (user == null)
            {
                throw new LoginFailedException(ApiResponseStatus.Error.ToString(), "User not found");
            }

            user.Email = dto.Email;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return new ApiResponse<object>
                {
                    Status = ApiResponseStatus.Error.ToString(),
                    Message = "Profile update failed",
                    Data = result.Errors.Select(e => e.Description)
                };
            }

            return new ApiResponse<object>
            {
                Status = ApiResponseStatus.Success.ToString(),
                Message = "Profile updated successfully"
            };
        }

        public async Task<ApiResponse<object>> UpdateUserProfileAsync(UpdateUserProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.Id);
            if (user == null)
            {
                throw new LoginFailedException(ApiResponseStatus.Error.ToString(), "User not found");
            }

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return new ApiResponse<object>
                {
                    Status = ApiResponseStatus.Error.ToString(),
                    Message = "Profile update failed",
                    Data = result.Errors.Select(e => e.Description)
                };
            }

            var userAreas = await _userAreasRepository.GetUserAreasByIdAsync(user.Id);
            userAreas.AllowedAreaIds = dto.AllowedAreas;
            await _userAreasRepository.UpdateUserAreasAsync(userAreas);

            return new ApiResponse<object>
            {
                Status = ApiResponseStatus.Success.ToString(),
                Message = "Profile updated successfully"
            };
        }

        public async Task<ApiResponse<object>> GetAllUsersAsync(Guid currentUserId)
        {
            var users = _userManager.Users.Where(u => u.Id != currentUserId).ToList();

            var userList = new List<UserWithRolesDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userRole = roles.FirstOrDefault() ?? "Unknown"; // Default to "Unknown" if no role is assigned

                userList.Add(new UserWithRolesDto
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    LastLogin = user.LastLogin,
                    Role = userRole
                });
            }

            return new ApiResponse<object>
            {
                Status = ApiResponseStatus.Success.ToString(),
                Message = "Users retrieved successfully",
                Data = userList
            };
        }


        public async Task<ApiResponse<object>> GetUserData(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return new ApiResponse<object>
                {
                    Status = ApiResponseStatus.Error.ToString(),
                    Message = "User not found"
                };
            }
            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault() ?? "Unknown"; // Default to "Unknown" if no role is assigned

            if (userRole == "Admin")
            {
                var userDto = new AdminUserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    ProfilePictureUrl = user.ProfilePictureUrl
                };
                return new ApiResponse<object>
                {
                    Status = ApiResponseStatus.Success.ToString(),
                    Message = "User data retrieved successfully",
                    Data = userDto
                };
            }
            else
            {
                var allowedAreas = await _userAreasRepository.GetUserAreasByIdAsync(user.Id);
                var userDto = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    AllowedAreas = allowedAreas.AllowedAreaIds,
                    ProfilePictureUrl = user.ProfilePictureUrl
                };
                return new ApiResponse<object>
                {
                    Status = ApiResponseStatus.Success.ToString(),
                    Message = "User data retrieved successfully",
                    Data = userDto
                };
            }
        }

        public async Task<ApiResponse<object>> UpdateUserPasswordAsync(UpdatePasswordDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null)
            {
                return new ApiResponse<object>
                {
                    Status = ApiResponseStatus.Error.ToString(),
                    Message = "Password Reset Failed"
                };
            }

            var TotpValid = _totpService.ValidateTotpCode(user.TOTPSecret, dto.Totp);

            if (!TotpValid)
            {
                return new ApiResponse<object>
                {
                    Status = ApiResponseStatus.Error.ToString(),
                    Message = "Invalid TOTP"
                };
            }

            var roken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, roken, dto.NewPassword);

            return new ApiResponse<object>
            {
                Status = result.Succeeded ? ApiResponseStatus.Success.ToString() : ApiResponseStatus.Error.ToString(),
                Message = result.Succeeded ? "Password Reset Successful" : "Password Reset Failed"
            };
        }

        public async Task<ApiResponse<object>> GetProfilePictureAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new ApiResponse<object>
                {
                    Status = ApiResponseStatus.Error.ToString(),
                    Message = "User not found"
                };
            }

            if (string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                return new ApiResponse<object>
                {
                    Status = ApiResponseStatus.Error.ToString(),
                    Message = "Profile picture not found"
                };
            }

            return new ApiResponse<object>
            {
                Status = ApiResponseStatus.Success.ToString(),
                Message = "Profile picture URL retrieved successfully",
                Data = new { ProfilePictureUrl = user.ProfilePictureUrl } // return the relative url.
            };
        }

        public async Task<ApiResponse<object>> UpdateProfilePictureAsync(UpdateProfilePictureDto dto, string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                return new ApiResponse<object>
                {
                    Status = ApiResponseStatus.Error.ToString(),
                    Message = "User not found"
                };
            }

            if (dto.ProfilePicture == null || dto.ProfilePicture.Length == 0)
            {
                return new ApiResponse<object>
                {
                    Status = ApiResponseStatus.Error.ToString(),
                    Message = "Invalid profile picture"
                };
            }

            // Remove existing profile picture
            RemoveProfilePicture(UserId);

            // Save new profile picture and get the relative URL
            string relativeUrl = SaveProfilePicture(dto.ProfilePicture, UserId);

            if (string.IsNullOrEmpty(relativeUrl))
            {
                return new ApiResponse<object>
                {
                    Status = ApiResponseStatus.Error.ToString(),
                    Message = "Failed to save profile picture"
                };
            }

            // Update user's profile picture URL
            user.ProfilePictureUrl = relativeUrl; // store the relative URL
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return new ApiResponse<object>
                {
                    Status = ApiResponseStatus.Error.ToString(),
                    Message = "Failed to update user profile"
                };
            }

            return new ApiResponse<object>
            {
                Status = ApiResponseStatus.Success.ToString(),
                Message = "Profile picture updated successfully",
                Data = new { ProfilePictureUrl = relativeUrl } // Return the URL in the response
            };
        }

        private string SaveProfilePicture(IFormFile profilePicture, string userId)
        {
            try
            {
                if (profilePicture.Length > 0)
                {
                    if (!Directory.Exists(ProfilePicturesPath))
                    {
                        Directory.CreateDirectory(ProfilePicturesPath);
                    }

                    string fileExtension = Path.GetExtension(profilePicture.FileName);
                    string fileName = userId + fileExtension;
                    string filePath = Path.Combine(ProfilePicturesPath, fileName);

                    using (FileStream fileStream = File.Create(filePath))
                    {
                        profilePicture.CopyTo(fileStream);
                        fileStream.Flush();
                    }

                    // Return the relative URL (important for web access)
                    return $"/ProfilePictures/{fileName}";
                }

                return null;
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred while saving profile picture for user {userId}: {ex}");
                return null;
            }
        }

        private bool RemoveProfilePicture(String UserId)
        {
            try
            {
                var files = Directory.EnumerateFiles(ProfilePicturesPath, UserId + ".*");
                var filePath = files.FirstOrDefault();

                if (filePath != null && File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                return true;
            }
            catch (System.Exception ex)
            {
                Log.Error($"an error happened while Deleting invoice for transaction with id of \"{UserId}\"\n{ex}");
                return false;
            }
        }
    }
}
