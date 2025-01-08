using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SmartHome.Shared.Models.Auth;
using SmartHome.Shared.Services.Auth.Interface;

namespace SmartHome.Shared.Services.Auth.Service
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        //private readonly IJwtStorageService _jwtStorageService;

        public AuthService(
            IHttpClientFactory httpClientFactory
            //IJwtStorageService jwtStorageService
            )
        {
            _httpClient = httpClientFactory.CreateClient("AuthClient");
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new LowercaseNamingPolicy(),
                WriteIndented = true
            };
            //_jwtStorageService = jwtStorageService;
        }

        public async Task<UserInfoDto?> GetCurrentUser()
        {
            //var token = await _jwtStorageService.GetTokenAsync();
            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var result = await _httpClient.GetFromJsonAsync<UserInfoDto>("/api/auth/userinfo");
            return result;
        }

        public async Task<HttpResponseMessage> Login(LoginDto Dto)
        {
            var jsonData = JsonSerializer.Serialize(Dto, _jsonOptions);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync("/api/auth/login", content);
            return result;
        }

        public Task<bool> Logout()
        {
            throw new NotImplementedException();
        }
    }
}
