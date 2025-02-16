using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Application.Interfaces.DeviceFunction
{
    public interface IDeviceFunctionRepository
    {
        Task<List<Domain.Entities.DeviceFunction>> GetDeviceFunctions();
        Task<Domain.Entities.DeviceFunction> GetDeviceFunction(Guid id);
        Task CreateDeviceFunction(Domain.Entities.DeviceFunction createDeviceFunctionDto);
        Task UpdateDeviceFunction(Domain.Entities.DeviceFunction updateDeviceFunctionDto);
        Task DeleteDeviceFunction(Domain.Entities.DeviceFunction deleteDeviceFunction);
    }
}
