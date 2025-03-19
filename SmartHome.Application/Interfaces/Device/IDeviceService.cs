using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Dto.Command;
using SmartHome.Dto.Device;
using SmartHome.Dto.Sensors;

namespace SmartHome.Application.Interfaces.Device
{
    public interface IDeviceService
    {
        Task<List<DeviceDto>> GetDevices();
        Task<List<DeviceDto>> GetDevicesForAreas(List<Guid> areaIds);
        Task<List<DeviceDto>> GetDevicesByArea(Guid Id);
        Task<DeviceDto> GetDevice(Guid id);
        Task CreateDevice(CreateDeviceDto createDeviceDto);
        Task UpdateDevice(UpdateDeviceDto updateDeviceDto);
        Task UpdateDeviceStateFromResponseAsync(DeviceResponseDto deviceResponse);
        Task UpdateSensorDataAsync(SensorDataDto sensorData);
        Task DeleteDevice(DeleteDeviceDto deleteDevice);
    }
}
