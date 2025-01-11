using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using SmartHome.Shared.Interfaces;
using SmartHome.Shared.Models;
using SmartHome.Shared.Models.Auth;

namespace SmartHome.Shared.Providers
{
    public class JwtAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IAuthService _api;
        private readonly IJwtStorageService _jwtStorageService;
        UserInfoDto _currentUser = new();

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
            var identity = new ClaimsIdentity();
            try
            {
                var userInfo = await GetCurrentUser();
                if (userInfo.IsAuthenticated)
                {
                    var claims = new[] { new Claim(ClaimTypes.Name, _currentUser.UserName) }.Concat(_currentUser.Claims.Select(c => new Claim(c.Key, c.Value)));
                    identity = new ClaimsIdentity(claims, "jwt");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Request failed:" + ex.ToString());
            }
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public async Task<bool> Logout()
        {
            //var result = await _api.Logout();
            _currentUser = new();
            await _jwtStorageService.RemoveTokenAsync();
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            return true;
        }

        public async Task<HttpResponseMessage> Login(LoginDto loginParameters)
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

        private async Task<UserInfoDto> GetCurrentUser()
        {
            if (_currentUser.IsAuthenticated) return _currentUser;

            _currentUser = await _api.GetCurrentUser();
            return _currentUser;
        }
    }

}
