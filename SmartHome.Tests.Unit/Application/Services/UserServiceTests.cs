using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using SmartHome.Application.Exceptions;
using SmartHome.Application.Interfaces.Jwt;
using SmartHome.Application.Interfaces.User;
using SmartHome.Application.Services.User;
using SmartHome.Domain.Entities;
using SmartHome.Dto;
using SmartHome.Dto.User;
using SmartHome.Enum;
using Xunit;

namespace SmartHome.Tests.Unit.Application.Services
{
    public class UserServiceTests
    {
        // Helper method to create a mock UserManager<ApplicationUser>
        private Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var options = Options.Create(new IdentityOptions());
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            var userValidators = new List<IUserValidator<ApplicationUser>> { new UserValidator<ApplicationUser>() };
            var pwdValidators = new List<IPasswordValidator<ApplicationUser>> { new PasswordValidator<ApplicationUser>() };
            var keyNormalizer = new UpperInvariantLookupNormalizer();
            var errors = new IdentityErrorDescriber();
            var services = new Mock<IServiceProvider>().Object;
            var logger = new Mock<Microsoft.Extensions.Logging.ILogger<UserManager<ApplicationUser>>>();

            return new Mock<UserManager<ApplicationUser>>(
                store.Object, options, passwordHasher, userValidators, pwdValidators, keyNormalizer, errors, services, logger.Object);
        }

        [Fact]
        public async Task CreateAdminUserAsync_ShouldReturnSuccess_WhenUserCreatedSuccessfully()
        {
            // Arrange
            var loginValidatorMock = new Mock<IValidator<LoginDto>>();
            var userManagerMock = CreateUserManagerMock();
            var jwtServiceMock = new Mock<IJwtTokenService>();
            var mapperMock = new Mock<IMapper>();

            // When CreateAsync is called, simulate success and assign an Id.
            userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<ApplicationUser, string>((user, password) => user.Id = new Guid());

            userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var userService = new UserService(
                loginValidatorMock.Object,
                userManagerMock.Object,
                jwtServiceMock.Object,
                mapperMock.Object);

            var registerDto = new RegisterAdminUserDto
            {
                Username = "adminUser",
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@example.com",
                Password = "Password123!"
            };

            // Act
            var response = await userService.CreateAdminUserAsync(registerDto);

            // Assert
            Assert.Equal(ApiResponseStatus.Success.ToString(), response.Status);
            Assert.Equal("User created successfully", response.Message);
            Assert.NotNull(response.Data);
            // Use dynamic to access the anonymous object returned in Data.
            //dynamic data = response.Data;
            //Assert.Equal("test-user-id", data.Id);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var loginValidatorMock = new Mock<IValidator<LoginDto>>();
            // Simulate a valid login by returning an empty ValidationResult (no errors).
            loginValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<LoginDto>(), default))
                .ReturnsAsync(new ValidationResult());

            var user = new ApplicationUser
            {
                Id = new Guid(),
                UserName = "testuser",
                Email = "test@example.com"
            };

            var userManagerMock = CreateUserManagerMock();
            userManagerMock.Setup(um => um.FindByNameAsync("testuser"))
                .ReturnsAsync(user);
            userManagerMock.Setup(um => um.CheckPasswordAsync(user, "Password123!"))
                .ReturnsAsync(true);
            userManagerMock.Setup(um => um.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "Admin" });

            var jwtServiceMock = new Mock<IJwtTokenService>();
            jwtServiceMock.Setup(js => js.GenerateJwtToken(user.Id.ToString(), user.UserName, user.Email, It.IsAny<IList<string>>()))
                .Returns("fake-jwt-token");

            var mapperMock = new Mock<IMapper>();

            var userService = new UserService(
                loginValidatorMock.Object,
                userManagerMock.Object,
                jwtServiceMock.Object,
                mapperMock.Object);

            var loginDto = new LoginDto
            {
                Username = "testuser",
                Password = "Password123!"
            };

            // Act
            var response = await userService.LoginAsync(loginDto);

            // Assert
            Assert.Equal(ApiResponseStatus.Success.ToString(), response.Status);
            Assert.Equal("Login Successful", response.Message);
            Assert.NotNull(response.Data);
            // Use dynamic to access the anonymous object returned in Data.
            var data = JsonSerializer.Deserialize<LoginResponseData>(JsonSerializer.Serialize(response.Data));
            Assert.Equal("fake-jwt-token", data.Token);
        }

        [Fact]
        public async Task UpdateUserProfileAsync_ShouldReturnSuccess_WhenUserIsUpdatedSuccessfully()
        {
            // Arrange
            var loginValidatorMock = new Mock<IValidator<LoginDto>>();
            var userManagerMock = CreateUserManagerMock();
            var jwtServiceMock = new Mock<IJwtTokenService>();
            var mapperMock = new Mock<IMapper>();
            var userId = new Guid();

            var user = new ApplicationUser
            {
                Id = userId,
                Email = "old@example.com",
                FirstName = "OldFirst",
                LastName = "OldLast"
            };

            userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);
            userManagerMock.Setup(um => um.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            var userService = new UserService(
                loginValidatorMock.Object,
                userManagerMock.Object,
                jwtServiceMock.Object,
                mapperMock.Object);

            var updateDto = new UpdateUserDto
            {
                UserId = new Guid().ToString(),
                Email = "new@example.com",
                FirstName = "NewFirst",
                LastName = "NewLast"
            };

            // Act
            var response = await userService.UpdateUserProfileAsync(updateDto);

            // Assert
            Assert.Equal(ApiResponseStatus.Success.ToString(), response.Status);
            Assert.Equal("Profile updated successfully", response.Message);
            // Also check that the user's properties were updated.
            Assert.Equal("new@example.com", user.Email);
            Assert.Equal("NewFirst", user.FirstName);
            Assert.Equal("NewLast", user.LastName);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldReturnSuccess_WhenUserIsDeletedSuccessfully()
        {
            // Arrange
            var loginValidatorMock = new Mock<IValidator<LoginDto>>();
            var userManagerMock = CreateUserManagerMock();
            var jwtServiceMock = new Mock<IJwtTokenService>();
            var mapperMock = new Mock<IMapper>();

            // Create a Guid and use its string representation as the user Id.
            var guid = Guid.NewGuid();
            var user = new ApplicationUser
            {
                Id = guid,
                Email = "test@example.com",
                UserName = "testuser"
            };

            userManagerMock.Setup(um => um.FindByIdAsync(guid.ToString()))
                .ReturnsAsync(user);
            userManagerMock.Setup(um => um.DeleteAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            var userService = new UserService(
                loginValidatorMock.Object,
                userManagerMock.Object,
                jwtServiceMock.Object,
                mapperMock.Object);

            // Act
            var response = await userService.DeleteUserAsync(guid);

            // Assert
            Assert.Equal(ApiResponseStatus.Success.ToString(), response.Status);
            Assert.Equal("User deleted successfully", response.Message);
        }
    }
}
