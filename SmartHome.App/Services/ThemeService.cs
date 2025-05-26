using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Shared.Interfaces;

namespace SmartHome.App.Services
{
    public class ThemeService : IThemeService
    {
        private readonly ISettingsService _settingsService;

        public ThemeService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        private readonly string _themeRecordName = "theme";

        public event Action? OnThemeChanged;

        public async Task<string?> GetThemeAsync()
        {
            var theme = "default"; // Default theme
            // Logic to retrieve the theme from user settings or application settings
            var userTheme = await SecureStorage.GetAsync(_themeRecordName);
            if (!string.IsNullOrEmpty(userTheme))
            {
                theme = userTheme;
            }
            else
            {
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
            await SecureStorage.SetAsync(_themeRecordName, theme);
            OnThemeChanged?.Invoke();
        }
    }
}
