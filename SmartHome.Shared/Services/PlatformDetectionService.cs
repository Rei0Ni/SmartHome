using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using SmartHome.Shared.Interfaces;

namespace SmartHome.Shared.Services
{
    public class PlatformDetectionService : IPlatformDetectionService
    {
        private readonly IJSRuntime _jsRuntime;

        public PlatformDetectionService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<bool> IsMobileAsync()
        {
            try
            {
                var module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/platformDetection.js");
                return await module.InvokeAsync<bool>("isMobile");
            }
            catch (Exception ex)
            {
                // Log exception if needed
                return false;
            }
        }
    }
}
