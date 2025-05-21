using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Dto.Settings;
using SmartHome.Shared.Interfaces;
using TimeZoneConverter;

namespace SmartHome.App.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly IApiService _apiService;
        private readonly ISecureStorageService _secureStorageService;
        public string? SystemTimeZone { get; private set; }
        public string? GlobalTheme { get; private set; }
        public event Action? OnSettingsChanged;

        public SettingsService(IApiService apiService, ISecureStorageService secureStorageService)
        {
            _apiService = apiService;
            _secureStorageService = secureStorageService;
        }

        private async Task<bool> AreHostsConfiguredAsync()
        {
            var (primaryHost, secondaryHost) = await _secureStorageService.GetHostnamesAsync();
            return !string.IsNullOrWhiteSpace(primaryHost) || !string.IsNullOrWhiteSpace(secondaryHost);
        }

        public async Task LoadSettingsAsync()
        {
            if (!await AreHostsConfiguredAsync())
            {
                return;
            }

            var response = await _apiService.GetAsync("api/settings/get/all");
            if (response.IsSuccessStatusCode)
            {
                var settings = await response.Content.ReadFromJsonAsync<List<SettingsDto>>();
                SystemTimeZone = settings?.FirstOrDefault(s => s.Key == "SystemTimeZone")?.Value ?? "Etc/UTC";
                GlobalTheme = settings?.FirstOrDefault(s => s.Key == "GlobalTheme")?.Value ?? "Light";
                OnSettingsChanged?.Invoke(); // Notify listeners 
            }
        }

        public async Task<bool> UpdateSettingsAsync(string timeZone, string theme)
        {
            if (!await AreHostsConfiguredAsync())
            {
                return false;
            }

            var dto = new SaveSettingsDto
            {
                Settings = new List<SettingsDto>
               {
                   new() { Key = "SystemTimeZone", Value = timeZone },
                   new() { Key = "GlobalTheme", Value = theme }
               }
            };
            var response = await _apiService.PostAsync("api/settings/update", dto);
            if (response.IsSuccessStatusCode)
            {
                SystemTimeZone = timeZone;
                GlobalTheme = theme;
                OnSettingsChanged?.Invoke();
                return true;
            }
            return false;
        }

        public async Task<TimeZoneInfo> GetSystemTimeZone()
        {
            var timeZoneId = SystemTimeZone;

            string platformSpecificId;

            if (OperatingSystem.IsWindows())
            {
                if (TZConvert.TryIanaToWindows(timeZoneId, out string windowsId))
                {
                    platformSpecificId = windowsId;
                }
                else
                {
                    Console.WriteLine($"Warning: Could not convert IANA ID '{timeZoneId}' to a Windows ID. Attempting fallback for UTC on Windows.");
                    // Fallback to a known Windows ID for UTC if conversion fails
                    platformSpecificId = TZConvert.IanaToWindows("Etc/UTC");
                }
            }
            else // For Linux, macOS, Android, iOS - they use IANA IDs.
            {
                platformSpecificId = timeZoneId;
            }

            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(platformSpecificId);
            }
            catch (TimeZoneNotFoundException ex)
            {
                Console.WriteLine($"ERROR: TimeZoneId '{platformSpecificId}' (derived from IANA '{timeZoneId}') not found on current system. {ex.Message}.");
            }
            catch (InvalidTimeZoneException ex) // Handles corrupted tzdata file issue
            {
                Console.WriteLine($"ERROR: TimeZoneId '{platformSpecificId}' (derived from IANA '{timeZoneId}') is invalid or tzdata corrupt. {ex.Message}.");
            }
            catch (Exception ex) // Other unexpected issues
            {
                Console.WriteLine($"ERROR: Unexpected error finding TimeZoneId '{platformSpecificId}'. {ex.Message}.");
            }

            try
            {
                return await Task.FromResult(TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving timezone: {ex.Message}");
            }
        }

        public async Task<string> GetSystemTheme()
        {
            return await Task.FromResult(GlobalTheme ?? "Light");
        }
    }
}
