using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using SmartHome.Application.Interfaces.Device;
using SmartHome.Domain.Contexts;
using SmartHome.Domain.Entities;
using SmartHome.Dto.Area;

namespace SmartHome.Infrastructure.Repositories
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly ApplicationDBContext _context;
        public DeviceRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task CreateDevice(Device createDeviceDto)
        {
            await _context.Devices.InsertOneAsync(createDeviceDto);
        }

        public async Task DeleteDevice(Device deleteDevice)
        {
            await _context.Devices.DeleteOneAsync(d => d.Id == deleteDevice.Id);
        }

        public async Task<Device> GetDevice(Guid id)
        {
            return await _context.Devices.Find(d => d.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Device>> GetDevices()
        {
            return await _context.Devices.Find(_ => true).ToListAsync();
        }

        public async Task<List<Device>> GetDevicesByArea(Guid Id)
        {
            return await _context.Devices.Find(x => x.AreaId == Id).ToListAsync();
        }

        public async Task<List<Device>> GetDevicesForAreas(List<Guid> areaIds)
        {
            return await _context.Devices
                            .Find(d => areaIds.Contains(d.AreaId))
                            .ToListAsync();
        }

        public async Task UpdateDevice(Device updateDeviceDto)
        {
            await _context.Devices.ReplaceOneAsync(d => d.Id == updateDeviceDto.Id, updateDeviceDto);
        }
    }
}
