using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using SmartHome.Application.Services;
using SmartHome.Domain.Entities;
using SmartHome.Enum;

namespace SmartHome.Tests.Unit.Application.Services
{
    public class SeedDataServiceTests
    {
        // Updated helper method to create a mock RoleManager<ApplicationRole>
        private Mock<RoleManager<ApplicationRole>> CreateRoleManagerMock()
        {
            var store = new Mock<IRoleStore<ApplicationRole>>();
            var roleValidators = new List<IRoleValidator<ApplicationRole>> { new RoleValidator<ApplicationRole>() };
            var keyNormalizer = new UpperInvariantLookupNormalizer();
            var errors = new IdentityErrorDescriber();
            var logger = new Mock<ILogger<RoleManager<ApplicationRole>>>();

            return new Mock<RoleManager<ApplicationRole>>(store.Object, roleValidators, keyNormalizer, errors, logger.Object);
        }

        // Updated helper method to create a mock UserManager<ApplicationUser>
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
            var logger = new Mock<ILogger<UserManager<ApplicationUser>>>();

            return new Mock<UserManager<ApplicationUser>>(
                store.Object, options, passwordHasher, userValidators, pwdValidators, keyNormalizer, errors, services, logger.Object);
        }

        // Helper method to get in-memory configuration for default admin password.
        private IConfiguration GetConfiguration(string defaultAdminPassword = null)
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                {"DefaultAdminPassword", defaultAdminPassword ?? "Administrator@2024"}
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        [Fact]
        public async Task InitializeAsync_CreatesRolesAndAdmin_WhenNotExisting()
        {
            // Arrange
            var roleManagerMock = CreateRoleManagerMock();
            var userManagerMock = CreateUserManagerMock();
            var configuration = GetConfiguration();

            // Assume that no roles exist, so RoleExistsAsync returns false for any role.
            roleManagerMock.Setup(rm => rm.RoleExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            // Simulate successful role creation.
            roleManagerMock.Setup(rm => rm.CreateAsync(It.IsAny<ApplicationRole>()))
                .ReturnsAsync(IdentityResult.Success);

            // Assume that the admin does not exist.
            userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser)null);
            // Simulate successful user creation.
            userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            // Simulate successful role assignment.
            userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var seedService = new SeedDataService(roleManagerMock.Object, userManagerMock.Object, configuration);

            // Act
            await seedService.InitializeAsync();

            // Assert
            // Verify that for each role in the enum, RoleExistsAsync and CreateAsync are called.
            var roles = System.Enum.GetNames(typeof(Role));
            foreach (var role in roles)
            {
                roleManagerMock.Verify(rm => rm.RoleExistsAsync(role), Times.Once);
                roleManagerMock.Verify(rm => rm.CreateAsync(It.Is<ApplicationRole>(ar => ar.Name == role)), Times.Once);
            }

            // Verify that the default administrator is created.
            string adminEmail = "Administrator@smarthome.com";
            userManagerMock.Verify(um => um.FindByEmailAsync(adminEmail), Times.Once);
            userManagerMock.Verify(um => um.CreateAsync(
                It.Is<ApplicationUser>(u => u.Email == adminEmail && u.UserName == "Administrator"),
                It.IsAny<string>()), Times.Once);
            userManagerMock.Verify(um => um.AddToRoleAsync(
                It.Is<ApplicationUser>(u => u.Email == adminEmail),
                System.Enum.GetName(typeof(Role), Role.Admin)), Times.Once);
        }

        [Fact]
        public async Task InitializeAsync_DoesNotCreateAdmin_WhenAlreadyExists()
        {
            // Arrange
            var roleManagerMock = CreateRoleManagerMock();
            var userManagerMock = CreateUserManagerMock();
            var configuration = GetConfiguration();

            // For this test, assume that roles already exist.
            roleManagerMock.Setup(rm => rm.RoleExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            // Simulate that the admin already exists.
            var existingAdmin = new ApplicationUser { Email = "Administrator@smarthome.com", UserName = "Administrator" };
            userManagerMock.Setup(um => um.FindByEmailAsync(existingAdmin.Email))
                .ReturnsAsync(existingAdmin);

            var seedService = new SeedDataService(roleManagerMock.Object, userManagerMock.Object, configuration);

            // Act
            await seedService.InitializeAsync();

            // Assert
            // Verify that the admin lookup was performed.
            userManagerMock.Verify(um => um.FindByEmailAsync(existingAdmin.Email), Times.Once);
            // Since the admin exists, no creation or role assignment should occur.
            userManagerMock.Verify(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
            userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }
    }
}
