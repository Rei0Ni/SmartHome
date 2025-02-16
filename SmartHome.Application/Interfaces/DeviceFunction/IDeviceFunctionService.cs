using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Dto.DeviceFunction;

namespace SmartHome.Application.Interfaces.DeviceFunction
{
    public interface IDeviceFunctionService
    {
        Task<List<DeviceFunctionDto>> GetDeviceFunctions();
        Task<DeviceFunctionDto> GetDeviceFunction(Guid id);
        Task CreateDeviceFunction(CreateDeviceFunctionDto createDeviceFunctionDto);
        Task UpdateDeviceFunction(UpdateDeviceFunctionDto updateDeviceFunctionDto);
        Task DeleteDeviceFunction(DeleteDeviceFunctionDto deleteDeviceFunction);
    }
}
