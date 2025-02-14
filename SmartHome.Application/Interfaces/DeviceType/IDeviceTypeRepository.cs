using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Application.Interfaces.DeviceType
{
    public interface IDeviceTypeRepository
    {
        Task<List<Domain.Entities.DeviceType>> GetDeviceTypes();
        Task<Domain.Entities.DeviceType> GetDeviceType(Guid id);
        Task CreateDeviceType(Domain.Entities.DeviceType createDeviceTypeDto);
        Task UpdateDeviceType(Domain.Entities.DeviceType updateDeviceTypeDto);
        Task DeleteDeviceType(Domain.Entities.DeviceType deleteDeviceType);
    }
}
