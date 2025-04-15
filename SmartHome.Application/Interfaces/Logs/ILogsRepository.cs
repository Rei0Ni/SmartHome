using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Domain.Entities;
using SmartHome.Enum;

namespace SmartHome.Application.Interfaces.Logs
{
    public interface ILogsRepository
    {
        Task AddLogAsync(Log log);
        Task<List<Log>> GetLogsAsync(int pageNumber, int pageSize);
        Task<List<Log>> GetLogsAsync(Guid userId, Guid areaId, int pageNumber, int pageSize);
        Task<List<Log>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber, int pageSize);
        Task<List<Log>> GetLogsByUserAsync(Guid userId, int pageNumber, int pageSize);
        Task<List<Log>> GetLogsByDeviceAsync(string deviceId, int pageNumber, int pageSize);
    }
}
