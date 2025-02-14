using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using SmartHome.Application.Interfaces.DeviceType;
using SmartHome.Domain.Contexts;
using SmartHome.Domain.Entities;
using SmartHome.Dto.Controller;

namespace SmartHome.Infrastructure.Repositories
{
    public class DeviceTypeRepository : IDeviceTypeRepository
    {
        public ApplicationDBContext _context { get; set; }
        public DeviceTypeRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task CreateDeviceType(DeviceType createDeviceTypeDto)
        {
            await _context.DeviceTypes.InsertOneAsync(createDeviceTypeDto);
        }

        public async Task DeleteDeviceType(DeviceType deleteDeviceType)
        {
            await _context.DeviceTypes.DeleteOneAsync(d => d.Id == deleteDeviceType.Id);
        }

        public async Task<DeviceType> GetDeviceType(Guid id)
        {
            return await _context.DeviceTypes.Find(d => d.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<DeviceType>> GetDeviceTypes()
        {
            return await _context.DeviceTypes.Find(_ => true).ToListAsync();
        }

        public async Task UpdateDeviceType(DeviceType updateDeviceTypeDto)
        {
            await _context.DeviceTypes.ReplaceOneAsync(d => d.Id == updateDeviceTypeDto.Id, updateDeviceTypeDto);
        }
    }
}
