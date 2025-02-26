using SmartHome.Shared.Interfaces;

namespace SmartHome.Web.Services
{
    public class SecureStorageService : ISecureStorageService
    {
        public async Task<(string?, string?)> GetHostnamesAsync()
        {
            // Simulate fetching hostnames from secure storage
            return await Task.FromResult<(string?, string?)>((null, null));
        }

        public async Task<bool> SetHostnamesAsync(string PrimaryHost, string SecondaryHost)
        {
            // Simulate setting hostnames in secure storage
            return await Task.FromResult(true);
        }
    }
}
