using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Domain.Entities;
using SmartHome.Dto.Email;

namespace SmartHome.Application.Interfaces.Settings
{
    public interface ISettingsRepository
    {
        Task<bool> SaveSettingsAsync(Setting entity);
        Task<bool> UpdateSettingsAsync(Setting entity);
        Task<List<Setting>> GetSettingsAsync();
        Task<bool> SettingsExistsAsync(string key);

        Task<EmailSetting> GetEmailSettingsAsync();
        Task SaveEmailSettingsAsync(EmailSetting dto);
        Task UpdateEmailSettingsAsync(EmailSetting dto);
    }
}
