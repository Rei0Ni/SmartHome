using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SmartHome.Application.Interfaces.Settings;
using SmartHome.Domain.Entities;
using SmartHome.Dto.Email;
using SmartHome.Dto.Settings;

namespace SmartHome.Application.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly IMapper _mapper;

        public SettingsService(ISettingsRepository settingsRepository, IMapper mapper)
        {
            _settingsRepository = settingsRepository;
            _mapper = mapper;
        }

        public async Task<EmailSettingsDto> GetEmailSettingsAsync()
        {
            var emailSettings = await _settingsRepository.GetEmailSettingsAsync();
            var emailSettingsDto = _mapper.Map<EmailSettingsDto>(emailSettings);
            return emailSettingsDto;
        }

        public async Task<List<SettingsDto>> GetSettingsAsync()
        {
            var settings = await _settingsRepository.GetSettingsAsync();
            var settingsDto = _mapper.Map<List<SettingsDto>>(settings);
            return settingsDto;
        }

        public async Task<bool> SaveSettingsAsync(SettingsDto setting)
        {
            var newSetting = _mapper.Map<Setting>(setting);
            return await _settingsRepository.SaveSettingsAsync(newSetting);
        }

        public async Task<bool> SettingsExistsAsync(string key)
        {
            return await _settingsRepository.SettingsExistsAsync(key);
        }

        public async Task UpdateEmailSettingsAsync(UpdateEmailSettingsDto dto)
        {
            var existingSettings = await _settingsRepository.GetEmailSettingsAsync();
            if (existingSettings != null)
            {
                existingSettings.Update(dto.SmtpServer, dto.Port, dto.SenderEmail, dto.SenderName, dto.Username, dto.Password, dto.UseSsl);
                await _settingsRepository.UpdateEmailSettingsAsync(existingSettings);
            }
            else
            {
                throw new InvalidOperationException("Email settings not found.");
            }
        }

        public async Task<bool> UpdateSettingsAsync(SettingsDto setting)
        {
            var newSetting = _mapper.Map<Setting>(setting);
            return await _settingsRepository.UpdateSettingsAsync(newSetting);
        }
    }
}
