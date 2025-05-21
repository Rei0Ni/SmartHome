using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using SmartHome.Application.Interfaces.Settings;
using SmartHome.Domain.Contexts;
using SmartHome.Domain.Entities;

namespace SmartHome.Infrastructure
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly ApplicationDBContext _context;

        public SettingsRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<EmailSetting> GetEmailSettingsAsync()
        {
            return await _context.EmailSettings.Find(_ => true).FirstOrDefaultAsync();
        }

        public async Task<List<Setting>> GetSettingsAsync()
        {
            var settings = await _context.Settings.Find(x => x.Key != null).ToListAsync();
            return settings;
        }

        public async Task SaveEmailSettingsAsync(EmailSetting dto)
        {
            await _context.EmailSettings.InsertOneAsync(dto);
        }

        public async Task<bool> SaveSettingsAsync(Setting entity)
        {
            try
            {
                await _context.Settings.InsertOneAsync(entity);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                return false;
            }
        }

        public async Task<bool> UpdateSettingsAsync(Setting entity)
        {
            try
            {
                var update = Builders<Setting>.Update
                    .Set(s => s.Value, entity.Value);

                var result = await _context.Settings.UpdateOneAsync(
                    s => s.Key == entity.Key,
                    update
                );

                return result.MatchedCount > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                return false;
            }
        }

        public async Task<bool> SettingsExistsAsync(string key)
        {
            return await _context.Settings.Find(s => s.Key == key).AnyAsync();
        }

        public async Task UpdateEmailSettingsAsync(EmailSetting dto)
        {
            var update = Builders<EmailSetting>.Update
                .Set(e => e.SmtpServer, dto.SmtpServer)
                .Set(e => e.Port, dto.Port)
                .Set(e => e.SenderEmail, dto.SenderEmail)
                .Set(e => e.SenderName, dto.SenderName)
                .Set(e => e.Username, dto.Username)
                .Set(e => e.Password, dto.Password)
                .Set(e => e.UseSsl, dto.UseSsl);

            await _context.EmailSettings.UpdateOneAsync(e => e.Id == dto.Id, update);
        }
    }
}
