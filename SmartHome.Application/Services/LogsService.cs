using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SmartHome.Application.Interfaces.Area;
using SmartHome.Application.Interfaces.Device;
using SmartHome.Application.Interfaces.Logs;
using SmartHome.Application.Interfaces.User;
using SmartHome.Domain.Entities;
using SmartHome.Dto.Log;
using SmartHome.Enum;

namespace SmartHome.Application.Services
{
    public class LogsService : ILogsService
    {
        private readonly ILogsRepository _logsRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IAreaRepository _areaRepository;
        public UserManager<ApplicationUser> _userManager;
        public IMapper _mapper;

        public LogsService(
            ILogsRepository logsRepository, 
            IDeviceRepository deviceRepository, 
            IAreaRepository areaRepository, 
            UserManager<ApplicationUser> userManager, 
            IMapper mapper)
        {
            _logsRepository = logsRepository;
            _deviceRepository = deviceRepository;
            _areaRepository = areaRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task AddLogAsync(string message, Guid deviceId, Guid userId, Guid areaId, LogLevel logLevel)
        {
            var device = await _deviceRepository.GetDevice(deviceId);
            var area = await _areaRepository.GetArea(areaId);
            var user = await _userManager.FindByIdAsync(userId.ToString());

            var log = new Log
            {
                Id = Guid.NewGuid(),
                Message = message,
                DeviceId = deviceId,
                AreaId = areaId,
                UserId = userId,
                DeviceName = device?.Name ?? "Unknown Device",
                AreaName = area?.Name ?? "Unknown Area",
                UserName = user?.UserName ?? "Unknown User",
                Level = logLevel
            };

            await _logsRepository.AddLogAsync(log);
        }

        public async Task<List<LogDto>> GetLogsAsync()
        {
            var logs = await _logsRepository.GetLogsAsync();
            return _mapper.Map<List<LogDto>>(logs);
        }

        public async Task<List<LogDto>> GetLogsAsync(Guid userId, Guid areaId, int pageNumber, int pageSize)
        {
            var logs = await _logsRepository.GetLogsAsync(userId, areaId, pageNumber, pageSize);
            return _mapper.Map<List<LogDto>>(logs);
        }

        public async Task<List<LogDto>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber, int pageSize)
        {
            var logs = await _logsRepository.GetLogsByDateRangeAsync(startDate, endDate, pageNumber, pageSize);
            return _mapper.Map<List<LogDto>>(logs);
        }

        public async Task<List<LogDto>> GetLogsByDeviceAsync(string deviceId, int pageNumber, int pageSize)
        {
            var logs = await _logsRepository.GetLogsByDeviceAsync(deviceId, pageNumber, pageSize);
            return _mapper.Map<List<LogDto>>(logs);
        }

        public async Task<List<LogDto>> GetLogsByUserAsync(Guid userId)
        {
            var logs = await _logsRepository.GetLogsByUserAsync(userId);
            return _mapper.Map<List<LogDto>>(logs);
        }
    }
}
