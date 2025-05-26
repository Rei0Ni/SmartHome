using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SmartHome.Shared.Interfaces;

namespace SmartHome.Web.Services
{
    public class JwtStorageService : IJwtStorageService
    {
        private readonly ProtectedLocalStorage _sessionStorage;
        private readonly string _jwtTokenRecordName = "JwtToken";

        public JwtStorageService(
            ProtectedLocalStorage sessionStorage
            )
        {

            _sessionStorage = sessionStorage;
        }
        public async Task<string?> GetTokenAsync()
        {
            var result = await _sessionStorage.GetAsync<string>(_jwtTokenRecordName);

            if (!string.IsNullOrEmpty(result.Value))
            {
                return result.Value;
            }

            return null;
        }

        public async Task RemoveTokenAsync()
        {
            await _sessionStorage.DeleteAsync(_jwtTokenRecordName);
        }

        public async Task SaveTokenAsync(string token)
        {
            await _sessionStorage.SetAsync(_jwtTokenRecordName, token);
        }
    }
}
