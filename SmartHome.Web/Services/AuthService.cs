using SmartHome.Dto.User;
using SmartHome.Shared.Interfaces;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SmartHome.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IJwtStorageService _jwtStorageService;

        public AuthService(
            IHttpClientFactory httpClientFactory,
            IJwtStorageService jwtStorageService
            )
        {
            _httpClient = httpClientFactory.CreateClient("AuthClient");
            _jwtStorageService = jwtStorageService;
        }

        public async Task<UserInfoDto?> GetCurrentUser()
        {
            var token = await _jwtStorageService.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var result = await _httpClient.GetFromJsonAsync<UserInfoDto>("/api/auth/userinfo");
            return result;
        }

        public async Task<HttpResponseMessage> Login(LoginDto Dto)
        {
            //var jsonData = JsonSerializer.Serialize(Dto, _jsonOptions);
            //var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsJsonAsync<LoginDto>("/api/auth/login", Dto);
            return result;
        }

        public Task<bool> Logout()
        {
            throw new NotImplementedException();
        }
    }
}
