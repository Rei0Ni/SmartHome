using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Dto.Settings;

namespace SmartHome.Shared.Interfaces
{
    public interface ISettingsService
    {
        Task LoadSettingsAsync();
        Task<TimeZoneInfo> GetSystemTimeZone();
        Task<string> GetSystemTheme();
        Task<bool> UpdateSettingsAsync(string timeZone, string theme);
    }
}
