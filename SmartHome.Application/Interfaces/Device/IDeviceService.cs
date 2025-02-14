using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Dto.Device;

namespace SmartHome.Application.Interfaces.Device
{
    public interface IDeviceService
    {
        Task<List<DeviceDto>> GetDevices();
        Task<DeviceDto> GetDevice(Guid id);
        Task CreateDevice(CreateDeviceDto createDeviceDto);
        Task UpdateDevice(UpdateDeviceDto updateDeviceDto);
        Task DeleteDevice(DeleteDeviceDto deleteDevice);
    }
}
