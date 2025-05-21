using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using SmartHome.Application.Interfaces.Logs;
using SmartHome.Domain.Contexts;
using SmartHome.Domain.Entities;
using SmartHome.Enum;

namespace SmartHome.Infrastructure.Repositories
{
    public class LogsRepository : ILogsRepository
    {
        private readonly ApplicationDBContext _context;

        public LogsRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task AddLogAsync(Log log)
        {
            await _context.Logs.InsertOneAsync(log);
        }

        public async Task<List<Log>> GetLogsAsync()
        {
            return await _context.Logs.Find(_ => true).ToListAsync();
        }

        public async Task<List<Log>> GetLogsAsync(Guid userId, Guid areaId, int pageNumber, int pageSize)
        {
            var filter = Builders<Log>.Filter.And(
                Builders<Log>.Filter.Eq(l => l.UserId, userId),
                Builders<Log>.Filter.Eq(l => l.AreaName, (await _context.Areas.Find(a => a.Id == areaId).FirstOrDefaultAsync())?.Name)
            );

            return await _context.Logs.Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task<List<Log>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber, int pageSize)
        {
            var filter = Builders<Log>.Filter.And(
                Builders<Log>.Filter.Gte(l => l.Timestamp, startDate),
                Builders<Log>.Filter.Lte(l => l.Timestamp, endDate)
            );

            return await _context.Logs.Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task<List<Log>> GetLogsByDeviceAsync(string deviceId, int pageNumber, int pageSize)
        {
            var filter = Builders<Log>.Filter.Eq(l => l.DeviceId, Guid.Parse(deviceId));

            return await _context.Logs.Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task<List<Log>> GetLogsByUserAsync(Guid userId)
        {
            var filter = Builders<Log>.Filter.Eq(l => l.UserId, userId);

            return await _context.Logs.Find(filter).ToListAsync();
        }
    }
}
