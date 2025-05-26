using SmartHome.Dto.Settings;
using SmartHome.Shared.Interfaces;
using TimeZoneConverter;

namespace SmartHome.Web.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly IApiService _apiService;
        private readonly IThemeService _themeService;

        public string? SystemTimeZone { get; private set; }
        public string? GlobalTheme { get; private set; }

        public event Action? OnSettingsChanged;

        public SettingsService(IApiService apiService, IThemeService themeService)
        {
            _apiService = apiService;
            _themeService = themeService;
        }

        public async Task LoadSettingsAsync()
        {
            var response = await _apiService.GetAsync("api/settings/get/all");
            var settings = await response.Content.ReadFromJsonAsync<List<SettingsDto>>();

            SystemTimeZone = settings?.FirstOrDefault(s => s.Key == "SystemTimeZone")?.Value ?? "UTC";
            GlobalTheme = settings?.FirstOrDefault(s => s.Key == "GlobalTheme")?.Value ?? "Light";

            OnSettingsChanged?.Invoke(); // Notify listeners
        }

        public async Task<bool> UpdateSettingsAsync(string timeZone, string theme)
        {
            var dto = new SaveSettingsDto
            {
                Settings = new List<SettingsDto>
                {
                    new() { Key = "SystemTimeZone", Value = TZConvert.WindowsToIana(timeZone)},
                    new() { Key = "GlobalTheme", Value = theme }
                }
            }; 

            await _themeService.SetThemeAsync(theme.ToLower()); // Update the theme in local storage

            var response = await _apiService.PostAsync("api/settings/update", dto);
            response.EnsureSuccessStatusCode();
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
            try
            {
                return await Task.FromResult(TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));
            }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.Utc; // Fallback to UTC if the timezone is not found
            }
            catch (InvalidTimeZoneException)
            {
                return TimeZoneInfo.Utc; // Fallback to UTC if the timezone is invalid
            }
        }

        public async Task<string> GetSystemTheme()
        {
            return await Task.FromResult(GlobalTheme ?? "Light");
        }
    }

}
