using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using SmartHome.Shared.Interfaces;
using Xamarin.Essentials;

namespace SmartHome.Shared.Services
{
    public class PlatformDetectionService : IPlatformDetectionService
    {
        private DevicePlatform devicePlatform = DeviceInfo.Platform;

        public bool IsMobile()
        {
            return devicePlatform == DevicePlatform.Android || devicePlatform == DevicePlatform.iOS; // (just for testinmng in windows) || devicePlatform == DevicePlatform.Unknown;
        }
    }
}
