using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Domain.Entities;
using SmartHome.Dto.Email;
using SmartHome.Dto.Settings;

namespace SmartHome.Application.Interfaces.Settings
{
    public interface ISettingsService
    {
        Task<bool> SaveSettingsAsync(SettingsDto setting);
        Task<bool> UpdateSettingsAsync(SettingsDto setting);
        Task<List<SettingsDto>> GetSettingsAsync();
        Task<bool> SettingsExistsAsync(string key);

        Task<EmailSettingsDto> GetEmailSettingsAsync();
        Task UpdateEmailSettingsAsync(UpdateEmailSettingsDto dto);
    }
}
