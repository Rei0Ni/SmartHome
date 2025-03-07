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

namespace SmartHome.Application.Services.User
{
    public class UserService : IUserService
    {
        private readonly IValidator<LoginDto> _loginValidator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IMapper _mapper;

        public UserService(
            IValidator<LoginDto> loginValidator,
            UserManager<ApplicationUser> userManager,
            IJwtTokenService jwtTokenService,
            IMapper mapper
            )
        {
            _loginValidator = loginValidator;
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _mapper = mapper;
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

            await _userManager.AddToRoleAsync(user, System.Enum.GetName(typeof(Role), Role.Normal_User)!);

            return new ApiResponse<object>
            {
                Status = ApiResponseStatus.Success.ToString(),
                Message = "User created successfully",
                Data = new { UserId = user.Id }
            };
        }

        public async Task<ApiResponse<object>> CreateGuestUserAsync(RegisterUserDto dto)
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

            await _userManager.AddToRoleAsync(user, System.Enum.GetName(typeof(Role), Role.Guest)!);

            return new ApiResponse<object>
            {
                Status = ApiResponseStatus.Success.ToString(),
                Message = "User created successfully",
                Data = new { UserId = user.Id }
            };
        }

        public async Task<ApiResponse<object>> CreateUserAsync(RegisterUserDto dto, Role role)
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

        public async Task<UserInfoDto> GetUserProfileAsync(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var applicationUser = _mapper.Map<UserInfoDto>(user);
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

            var response = new ApiResponse<object>
            {
                Status = ApiResponseStatus.Success.ToString(),
                Message = "Login Successful",
                Data = new
                {
                    Token = token
                }
            };
            return response;
        }

        public async Task<ApiResponse<object>> UpdateUserProfileAsync(UpdateUserDto dto)
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

    }
}
