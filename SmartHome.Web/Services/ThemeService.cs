using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SmartHome.Shared.Interfaces;

namespace SmartHome.Web.Services
{
    public class ThemeService : IThemeService
    {
        private readonly ProtectedLocalStorage _themeSettings;
        private readonly IServiceProvider _serviceProvider;



        public ThemeService(IServiceProvider serviceProvider, ProtectedLocalStorage themeSettings)
        {
            _serviceProvider = serviceProvider;
            _themeSettings = themeSettings;
        }

        private readonly string _themeRecordName = "theme";

        public event Action? OnThemeChanged;

        public async Task<string?> GetThemeAsync()
        {
            var theme = "default"; // Default theme
            // Logic to retrieve the theme from user settings or application settings
            var userTheme = await _themeSettings.GetAsync<string>(_themeRecordName);
            if (userTheme.Success)
            {
                theme = userTheme.Value;
            }
            else
            {
                var _settingsService = _serviceProvider.GetRequiredService<ISettingsService>();

                await _settingsService.LoadSettingsAsync();
                var systemTheme = await _settingsService.GetSystemTheme();
                if (!string.IsNullOrEmpty(systemTheme))
                {
                    theme = systemTheme;
                }
            }
            return theme;
        }
        public async Task SetThemeAsync(string theme)
        {
            await _themeSettings.SetAsync(_themeRecordName, theme);
            OnThemeChanged?.Invoke();
        }
    }
}
