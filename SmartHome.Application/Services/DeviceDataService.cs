using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SmartHome.Application.Interfaces.Device;
using SmartHome.Application.Interfaces.DeviceType;
using SmartHome.Application.Interfaces.IPCameras;
using SmartHome.Dto.Device;
using SmartHome.Dto.IPCamera;

namespace SmartHome.Application.Services
{
    public class DeviceDataService : IDeviceDataService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IIPCamerasRepository _camerasRepository;
        private readonly IMapper _mapper;
        private readonly IDeviceTypeService _deviceTypeService;

        public DeviceDataService(
            IDeviceRepository deviceRepository, 
            IMapper mapper, 
            IDeviceTypeService deviceTypeService, 
            IIPCamerasRepository camerasRepository)
        {
            _deviceRepository = deviceRepository;
            _mapper = mapper;
            _deviceTypeService = deviceTypeService;
            _camerasRepository = camerasRepository;
        }

        public async Task<List<DeviceDto>> GetDevicesForAreas(List<Guid> areaIds)
        {
            var devices = await _deviceRepository.GetDevicesForAreas(areaIds);
            var deviceDtos = _mapper.Map<List<DeviceDto>>(devices);

            // Fetch related data in bulk for performance
            var deviceTypeIds = devices.Select(d => d.DeviceTypeId).Distinct().ToList();
            var deviceTypes = await _deviceTypeService.GetDeviceTypes();

            foreach (var dto in deviceDtos)
            {
                var device = devices.FirstOrDefault(d => d.Id == dto.Id);
                if (device == null) continue;

                var deviceType = deviceTypes.FirstOrDefault(dt => dt.Id == device.DeviceTypeId);
                if (deviceType != null)
                {
                    dto.DeviceType = deviceType;
                }
            }
            return deviceDtos;
        }

        public async Task<List<IPCameraDto>> GetIPCamerasForAreas(List<Guid> areaIds)
        {
            var cameras = await _camerasRepository.GetCamerasForAreas(areaIds);
            var cameraDtos = _mapper.Map<List<IPCameraDto>>(cameras);
            return cameraDtos;
        }
    }

}
