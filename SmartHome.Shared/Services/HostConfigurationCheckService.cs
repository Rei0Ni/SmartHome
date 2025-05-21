using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Shared.Interfaces;
using Xamarin.Essentials;

namespace SmartHome.Shared.Services
{
    public class HostConfigurationCheckService : IHostConfigurationCheckService
    {
        private readonly ISecureStorageService _secureStorageService;
        private readonly IPlatformDetectionService _platformDetectionService;


        public HostConfigurationCheckService(ISecureStorageService secureStorageService, IPlatformDetectionService platformDetectionService)
        {
            _secureStorageService = secureStorageService;
            _platformDetectionService = platformDetectionService;
        }

        public async Task<bool> AreHostConfigurationsPresentAsync()
        {
            var Hostnames = await _secureStorageService.GetHostnamesAsync();
            return !string.IsNullOrEmpty(Hostnames.Item1) && !string.IsNullOrEmpty(Hostnames.Item2);
        }

        public async Task<bool> ShouldNavigateToConfigurationPageAsync()
        {
            bool isMobile = _platformDetectionService.IsMobile();
            if (isMobile)
            {
                return !await AreHostConfigurationsPresentAsync();
            }
            return false; // Not mobile, no need to navigate
        }
    }
}
