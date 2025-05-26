using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using SmartHome.Dto.User;
using SmartHome.Shared.Interfaces;
using SmartHome.Shared.Models;
using SmartHome.Shared.Models.Auth;

namespace SmartHome.Shared.Providers
{
    public class JwtAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IAuthService _api;
        private readonly IJwtStorageService _jwtStorageService;
        Dto.User.UserAuthenticationState _currentUserAuthenticationState = new();

        public JwtAuthStateProvider(
            IAuthService api,
            IJwtStorageService jwtStorageService
            )
        {
            _api = api;
            _jwtStorageService = jwtStorageService;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            await Task.Delay(1500); // Force the delay
            Console.WriteLine("[JwtAuthenticationProvider] Checking Authentication State");

            var identity = new ClaimsIdentity();
            try
            {
                var userInfo = await GetCurrentUserAuthenticationState();
                if (userInfo.IsAuthenticated)
                {
                    var claimsList = new List<Claim>();

                    // Add the username claim (or NameIdentifier if more appropriate)
                    if (!string.IsNullOrEmpty(_currentUserAuthenticationState.UserName))
                    {
                        claimsList.Add(new Claim(ClaimTypes.Name, _currentUserAuthenticationState.UserName));
                        // Consider adding NameIdentifier if your user ID is different from UserName
                        // claimsList.Add(new Claim(ClaimTypes.NameIdentifier, _currentUserAuthenticationState.UserId)); // Assuming UserId property exists
                    }

                    if (_currentUserAuthenticationState.Claims != null)
                    {
                        foreach (var claimPair in _currentUserAuthenticationState.Claims)
                        {
                            // This is the crucial part for roles:
                            // Check if the key from your backend signifies a role.
                            // Common keys are "role" or the full URI ClaimTypes.Role.
                            // Adjust "role" if your backend uses a different key (e.g., "userRole", "roles").
                            if (claimPair.Key.Equals("role", StringComparison.OrdinalIgnoreCase) ||
                                claimPair.Key.Equals(ClaimTypes.Role, StringComparison.OrdinalIgnoreCase))
                            {
                                // If the role value might contain multiple roles (e.g., "Admin,Editor"),
                                // you'll need to split claimPair.Value and add a ClaimTypes.Role for each.
                                // Example for comma-separated roles:
                                var roles = claimPair.Value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var role in roles)
                                {
                                    claimsList.Add(new Claim(ClaimTypes.Role, role.Trim()));
                                    Console.WriteLine($"[JwtAuthenticationProvider] Added Role Claim: {role.Trim()}");
                                }
                            }
                            else
                            {
                                // For other claims, add them with their original key
                                claimsList.Add(new Claim(claimPair.Key, claimPair.Value));
                            }
                        }
                    }

                    identity = new ClaimsIdentity(claimsList, "jwt"); // "jwt" is the authenticationType
                    Console.WriteLine($"[JwtAuthenticationProvider] User is Authenticated. Claims count: {claimsList.Count}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"[JwtAuthenticationProvider] Token Parsing Failed: {ex.Message}");
            }
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public async Task<bool> Logout()
        {
            //var result = await _api.Logout();
            _currentUserAuthenticationState = new();
            await _jwtStorageService.RemoveTokenAsync();
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            return true;
        }

        public async Task<HttpResponseMessage> Login(Dto.User.LoginDto loginParameters)
        {
            try
            {
                var result = await _api.Login(loginParameters);
                // save token to sessionstorage
                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadFromJsonAsync<ApiResponse<LoginSuccessResponse>>();
                    await _jwtStorageService.SaveTokenAsync(response.Data.Token);
                }
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine("Authentication Failure "+ e.Message);
                throw;
            }
            
        }

        private async Task<Dto.User.UserAuthenticationState> GetCurrentUserAuthenticationState()
        {
            if (_currentUserAuthenticationState.IsAuthenticated) return _currentUserAuthenticationState;

            try
            {
                var userState = await _api.GetCurrentUserAuthenticationState();
                if (userState != null)
                {
                    _currentUserAuthenticationState = userState;
                }
            }
            catch (Exception)
            {
                return _currentUserAuthenticationState;
            }
            return _currentUserAuthenticationState;
        }
    }

}
