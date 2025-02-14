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
    }
}
