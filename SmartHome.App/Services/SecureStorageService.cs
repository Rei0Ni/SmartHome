using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Shared.Interfaces;

namespace SmartHome.App.Services
{
    public class SecureStorageService : ISecureStorageService
    {
        private readonly string _primaryHostKey = "PrimaryHost";
        private readonly string _secondaryHostKey = "SecondaryHost";

        public async Task<(string?, string?)> GetHostnamesAsync()
        {
            string? PrimaryHost = await SecureStorage.GetAsync(key: _primaryHostKey);
            string? SecondaryHost = await SecureStorage.GetAsync(key: _secondaryHostKey);

            return (PrimaryHost, SecondaryHost);
        }

        public async Task<bool> SetHostnamesAsync(string PrimaryHost, string SecondaryHost)
        {
            try
            {
                // Ensure PrimaryHost has the correct port  
                PrimaryHost = ReplaceOrAppendPort(PrimaryHost, "62061");

                // Ensure SecondaryHost has the correct port  
                SecondaryHost = ReplaceOrAppendPort(SecondaryHost, "62061");

                await SecureStorage.SetAsync(key: _primaryHostKey, value: PrimaryHost);
                await SecureStorage.SetAsync(key: _secondaryHostKey, value: SecondaryHost);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string ReplaceOrAppendPort(string host, string port)
        {
            var uriBuilder = new UriBuilder(host);
            uriBuilder.Port = int.Parse(port);
            return uriBuilder.ToString();
        }
    }
}
