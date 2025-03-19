using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SmartHome.Application.Interfaces.Device;
using SmartHome.Application.Interfaces.DeviceFunction;
using SmartHome.Application.Interfaces.DeviceType;
using SmartHome.Dto.Device;

namespace SmartHome.Application.Services
{
    public class DeviceDataService : IDeviceDataService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IMapper _mapper;
        private readonly IDeviceTypeService _deviceTypeService;
        private readonly IDeviceFunctionService _deviceFunctionService;

        public DeviceDataService(IDeviceRepository deviceRepository, IMapper mapper, IDeviceTypeService deviceTypeService, IDeviceFunctionService deviceFunctionService)
        {
            _deviceRepository = deviceRepository;
            _mapper = mapper;
            _deviceTypeService = deviceTypeService;
            _deviceFunctionService = deviceFunctionService;
        }

        public async Task<List<DeviceDto>> GetDevicesForAreas(List<Guid> areaIds)
        {
            var devices = await _deviceRepository.GetDevicesForAreas(areaIds);
            var deviceDtos = _mapper.Map<List<DeviceDto>>(devices);

            // Fetch related data in bulk for performance
            var deviceTypeIds = devices.Select(d => d.DeviceTypeId).Distinct().ToList();
            var deviceTypes = await _deviceTypeService.GetDeviceTypes();
            var allDeviceFunctions = await _deviceFunctionService.GetDeviceFunctions();


            foreach (var dto in deviceDtos)
            {
                var device = devices.FirstOrDefault(d => d.Id == dto.Id);
                if (device == null) continue;

                var deviceType = deviceTypes.FirstOrDefault(dt => dt.Id == device.DeviceTypeId);
                if (deviceType != null)
                {
                    dto.DeviceType = deviceType;
                    dto.DeviceFunctions = allDeviceFunctions.Where(df => deviceType.Functions.Contains(df.Id)).ToList();
                }
            }
            return deviceDtos;
        }
    }

}
