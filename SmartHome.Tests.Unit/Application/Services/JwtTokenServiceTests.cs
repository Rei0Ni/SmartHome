using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using SmartHome.Application.Services;
using SmartHome.Enum;

namespace SmartHome.Tests.Unit.Application.Services
{
    public class JwtTokenServiceTests
    {
        // Helper method to create a configuration with JWT settings
        private IConfiguration Configuration
        {
            get
            {
                var inMemorySettings = new Dictionary<string, string>
                {
                    {"Jwt:Key", "supersecret_test_key_12345678900"}, // Ensure the key length is sufficient
                    {"Jwt:ExpirationDays", "1"},
                    {"Jwt:Issuer", "TestIssuer"},
                    {"Jwt:Audience", "TestAudience"}
                };

                return new ConfigurationBuilder()
                    .AddInMemoryCollection(inMemorySettings!)
                    .Build();
            }
        }

        [Fact]
        public void GenerateJwtToken_ValidInput_ReturnsValidToken()
        {
            // Arrange
            var configuration = Configuration;
            var jwtService = new JwtTokenService(configuration);
            var userId = new Guid().ToString();
            var username = "TestUser";
            var email = "test@example.com";
            var roles = new List<string> { 
                System.Enum.GetName(typeof(Role), Role.Admin)!,
                System.Enum.GetName(typeof(Role), Role.Normal_User)!
            };

            // Act
            var token = jwtService.GenerateJwtToken(userId, username, email, roles);

            // Assert: Ensure the token is not null or empty
            Assert.False(string.IsNullOrWhiteSpace(token));

            // Optionally, decode the token to verify its claims
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            Assert.Equal(userId, jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
            Assert.Equal(email, jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
            Assert.Equal(username, jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Name).Value);

            // Verify that each role is present as a claim
            var roleClaims = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
            Assert.Equal(roles.Count, roleClaims.Count);
            foreach (var role in roles)
            {
                Assert.Contains(roleClaims, rc => rc.Value == role);
            }
        }

        [Fact]
        public void ValidateJwtToken_ValidToken_DoesNotThrow()
        {
            // Arrange
            var configuration = Configuration;
            var jwtService = new JwtTokenService(configuration);
            var token = jwtService.GenerateJwtToken("123", "TestUser", "test@example.com", new List<string> 
            {
                System.Enum.GetName(typeof(Role), Role.Admin)!,
                System.Enum.GetName(typeof(Role), Role.Normal_User)!
            });

            // Act & Assert: Expect no exceptions for a valid token
            var exception = Record.Exception(() => jwtService.ValidateJwtToken(token));
            Assert.Null(exception);
        }

        [Fact]
        public void ValidateJwtToken_InvalidToken_ThrowsException()
        {
            // Arrange
            var configuration = Configuration;
            var jwtService = new JwtTokenService(configuration);
            var invalidToken = "this.is.an.invalid.token";

            // Act & Assert: Expect an exception when validating an invalid token
            Assert.ThrowsAny<Exception>(() => jwtService.ValidateJwtToken(invalidToken));
        }

        [Fact]
        public void GenerateJwtToken_MissingConfiguration_ThrowsException()
        {
            // Arrange: Create a configuration that is missing the Jwt:Key
            var inMemorySettings = new Dictionary<string, string>
            {
                // Jwt:Key is intentionally missing here
                {"Jwt:ExpirationDays", "1"},
                {"Jwt:Issuer", "TestIssuer"},
                {"Jwt:Audience", "TestAudience"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var jwtService = new JwtTokenService(configuration);

            // Act & Assert: Expect an ArgumentNullException since the Jwt:Key is missing.
            var ex = Assert.Throws<ArgumentNullException>(() =>
                jwtService.GenerateJwtToken("123", "TestUser", "test@example.com", new List<string>
                {
                    System.Enum.GetName(typeof(Role), Role.Admin)!,
                    System.Enum.GetName(typeof(Role), Role.Normal_User)!
                })
            );

            Assert.Contains("Value cannot be null", ex.Message);
        }

    }
}
