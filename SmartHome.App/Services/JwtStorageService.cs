using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Shared.Interfaces;

namespace SmartHome.App.Services
{
    public class JwtStorageService : IJwtStorageService
    {
        private readonly string _jwtTokenRecordName = "JwtToken";

        public JwtStorageService()
        {

        }
        public async Task<string?> GetTokenAsync()
        {
            var result = (string)(await SecureStorage.GetAsync(_jwtTokenRecordName))!;

            if (!String.IsNullOrEmpty(result))
            {
                return result;
            }

            return null;
        }

        public Task RemoveTokenAsync()
        {
            SecureStorage.Remove(_jwtTokenRecordName);
            return Task.CompletedTask;
        }

        public async Task SaveTokenAsync(string token)
        {
            await SecureStorage.SetAsync(_jwtTokenRecordName, token);
        }
    }
}
