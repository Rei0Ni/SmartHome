using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using SmartHome.Application.DTOs.User;
using SmartHome.Application.Exceptions;
using SmartHome.Application.Interfaces.Jwt;
using SmartHome.Application.Interfaces.User;
using SmartHome.Domain.Entities;

namespace SmartHome.Application.Services.User
{
    public class UserService : IUserService
    {
        private readonly IValidator<LoginDto> _loginValidator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;

        public UserService(
            IValidator<LoginDto> loginValidator,
            UserManager<ApplicationUser> userManager,
            IJwtTokenService jwtTokenService
            )
        {
            _loginValidator = loginValidator;
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            ValidationResult result = await _loginValidator.ValidateAsync(dto);

            if (!result.IsValid)
            {
                throw new ValidationException("Invalid Credentials", result.Errors);
            }

            var user = await _userManager.FindByNameAsync(dto.Username);

            if (user == null)
            {
                throw new LoginFailedException("Invalid Username or Password");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!passwordValid)
            {
                throw new LoginFailedException("Invalid Username or Password");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var token = _jwtTokenService.GenerateJwtToken(user.Id.ToString(), user.UserName!, user.Email!, userRoles);

            return token;
        }
    }
}
