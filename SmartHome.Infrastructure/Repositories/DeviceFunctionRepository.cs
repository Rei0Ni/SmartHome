using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using SmartHome.Application.Interfaces.DeviceFunction;
using SmartHome.Domain.Contexts;
using SmartHome.Domain.Entities;
using SmartHome.Dto.Area;
using SmartHome.Dto.Controller;

namespace SmartHome.Infrastructure.Repositories
{
    public class DeviceFunctionRepository : IDeviceFunctionRepository
    {
        public ApplicationDBContext _context { get; set; }

        public DeviceFunctionRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task CreateDeviceFunction(DeviceFunction createDeviceFunctionDto)
        {
            await _context.DeviceFunctions.InsertOneAsync(createDeviceFunctionDto);
        }

        public async Task DeleteDeviceFunction(DeviceFunction deleteDeviceFunction)
        {
            await _context.DeviceFunctions.DeleteOneAsync(df => df.Id == deleteDeviceFunction.Id);
        }

        public async Task<DeviceFunction> GetDeviceFunction(Guid id)
        {
            return await _context.DeviceFunctions.Find(df => df.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<DeviceFunction>> GetDeviceFunctions()
        {
            return await _context.DeviceFunctions.Find(_ => true).ToListAsync();
        }

        public async Task UpdateDeviceFunction(DeviceFunction updateDeviceFunctionDto)
        {
            await _context.DeviceFunctions.ReplaceOneAsync(df => df.Id == updateDeviceFunctionDto.Id, updateDeviceFunctionDto);
        }
    }
}
