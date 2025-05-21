using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Dto.Log;
using SmartHome.Enum;

namespace SmartHome.Application.Interfaces.Logs
{
    public interface ILogsService
    {
        Task AddLogAsync(string message, Guid deviceId, Guid userId, Guid areaId, LogLevel logLevel);
        Task<List<LogDto>> GetLogsAsync();
        Task<List<LogDto>> GetLogsAsync(Guid userId, Guid areaId, int pageNumber, int pageSize);
        Task<List<LogDto>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber, int pageSize);
        Task<List<LogDto>> GetLogsByUserAsync(Guid userId);
        Task<List<LogDto>> GetLogsByDeviceAsync(string deviceId, int pageNumber, int pageSize);
    }
}
