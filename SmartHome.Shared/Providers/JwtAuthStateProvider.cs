﻿using System;
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
                    var claims = new[] { new Claim(ClaimTypes.Name, _currentUserAuthenticationState.UserName) }.Concat(_currentUserAuthenticationState.Claims.Select(c => new Claim(c.Key, c.Value)));
                    identity = new ClaimsIdentity(claims, "jwt");
                    Console.WriteLine("[JwtAuthenticationProvider] User is Authenticated");
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
