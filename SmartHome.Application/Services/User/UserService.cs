using System;
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

namespace SmartHome.Application.Services.User
{
    public class UserService : IUserService
    {
        private readonly IValidator<LoginDto> _loginValidator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IMapper _mapper;
        private readonly IUserAreasRepository _userAreasRepository;

        public UserService(
            IValidator<LoginDto> loginValidator,
            UserManager<ApplicationUser> userManager,
            IJwtTokenService jwtTokenService,
            IMapper mapper,
            IUserAreasRepository userAreasRepository,
            IUserRepository userRepository)
        {
            _loginValidator = loginValidator;
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _mapper = mapper;
            _userAreasRepository = userAreasRepository;
            _userRepository = userRepository;
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

            return new ApiResponse<object>
            {
                Status = ApiResponseStatus.Success.ToString(),
                Message = "User created successfully",
                Data = new { UserId = user.Id }
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

            return new ApiResponse<object>
            {
                Status = ApiResponseStatus.Success.ToString(),
                Message = "User created successfully",
                Data = new { UserId = user.Id }
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
                throw new FluentValidationException(ApiResponseStatus.Error.ToString(),"Invalid Credentials", result.Errors);
            }

            var user = await _userManager.FindByNameAsync(dto.Username);

            if (user == null)
            {
                throw new LoginFailedException(ApiResponseStatus.Error.ToString(), "Invalid Username or Password");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!passwordValid)
            {
                throw new LoginFailedException(ApiResponseStatus.Error.ToString(),"Invalid Username or Password");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var token = _jwtTokenService.GenerateJwtToken(user.Id.ToString(), user.UserName!, user.Email!, userRoles);

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
            var user = await _userManager.FindByIdAsync(dto.UserId);
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
            var user = await _userManager.FindByIdAsync(dto.UserId);
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

            return new ApiResponse<object>
            {
                Status = ApiResponseStatus.Success.ToString(),
                Message = "Profile updated successfully"
            };
        }

        public async Task<ApiResponse<object>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();

            var userList = new List<UserWithRolesDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userRole = roles.FirstOrDefault() ?? "Unknown"; // Default to "Unknown" if no role is assigned

                userList.Add(new UserWithRolesDto
                {
                    UserId = user.Id.ToString(),
                    Username = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
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
    }
}
