using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SmartHome.Dto.User;
using SmartHome.Shared.Interfaces;

namespace SmartHome.App.Services
{
    public class AuthService : IAuthService
    {
        private readonly IApiService _apiService;
        private readonly IJwtStorageService _jwtStorageService;
        private readonly ISecureStorageService _secureStorageService;
        private readonly ILogger<AuthService> _logger; // Use the logger

        public AuthService(
            IJwtStorageService jwtStorageService,
            ISecureStorageService secureStorageService,
            ILogger<AuthService> logger, // Inject and use ILogger
            IApiService apiService)
        {
            _jwtStorageService = jwtStorageService;
            _secureStorageService = secureStorageService;
            _apiService = apiService;
            _logger = logger; // Assign the injected logger
        }

        public async Task<UserAuthenticationState?> GetCurrentUserAuthenticationState()
        {
            try
            {
                _logger.LogInformation("Getting current user info."); // Log action
                var token = await _jwtStorageService.GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("No JWT token found. User might not be logged in."); // Log warning
                    return new UserAuthenticationState(); // Or handle no token case as appropriate for your app
                }

                var result = await _apiService.GetAsync("/api/auth/authentication-state");

                var userInfo = await result.Content.ReadFromJsonAsync<UserAuthenticationState>();

                if (userInfo != null)
                {
                    _logger.LogDebug("Successfully retrieved user info for user: {UserId}", userInfo.Claims.Where(x => x.Key == ClaimTypes.Sid)); // Log success
                    return userInfo;
                }
                else
                {
                    _logger.LogWarning("Failed to retrieve user info. API might have returned null."); // Log warning
                    return null; // Or handle null response as needed
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting current user info."); // Log full exception
                return null; // Or handle error as appropriate
            }
        }

        public async Task<HttpResponseMessage> Login(LoginDto dto)
        {
            try
            {
                _logger.LogInformation("Attempting login for user: {UserName}", dto.Username); // Log action
                HttpResponseMessage response = await _apiService.PostAsync<LoginDto>("/api/auth/login", dto);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent);
                    _logger.LogDebug("Login successful for user: {UserName}. Status code: {StatusCode}", dto.Username, response.StatusCode); // Log success
                    return response;
                }
                else
                {
                    _logger.LogWarning("Login failed for user: {UserName}. Status code: {StatusCode}", dto.Username, response.StatusCode); // Log failure
                    return response; // Return the response even on failure, so the caller can handle error details
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login attempt for user: {UserName}", dto.Username); // Log full exception
                return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError); // Return generic error response
            }
        }

        public Task<bool> Logout()
        {
            _logger.LogInformation("Logout requested. (Not implemented)"); // Log action
            throw new NotImplementedException("Logout functionality is not yet implemented."); // Keep the NotImplementedException as it is
        }
    }
}