using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Dto.DeviceType;

namespace SmartHome.Application.Interfaces.DeviceType
{
    public interface IDeviceTypeService
    {
        Task<List<DeviceTypeDto>> GetDeviceTypes();
        Task<DeviceTypeDto> GetDeviceType(Guid id);
        Task CreateDeviceType(CreateDeviceTypeDto createDeviceTypeDto);
        Task UpdateDeviceType(UpdateDeviceTypeDto updateDeviceTypeDto);
        Task DeleteDeviceType(DeleteDeviceTypeDto deleteDeviceType);
    }
}
