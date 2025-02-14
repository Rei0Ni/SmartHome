using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SmartHome.Application.Interfaces;
using SmartHome.Application.Interfaces.Area;
using SmartHome.Application.Interfaces.Device;
using SmartHome.Application.Interfaces.DeviceFunction;
using SmartHome.Application.Interfaces.DeviceType;
using SmartHome.Dto.Dashboard;
using SmartHome.Dto.DeviceFunction;

namespace SmartHome.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IAreaService _areaService;
        private readonly IDeviceService _deviceService;
        private readonly IDeviceTypeService _deviceTypeService; // Add this line
        private readonly IDeviceFunctionService _deviceFunctionService; // Add this line
        private IMapper _mapper;

        public DashboardService(IAreaService areaService, IDeviceService deviceService, IDeviceTypeService deviceTypeService, IDeviceFunctionService deviceFunctionService, IMapper mapper) // Modify this line
        {
            _areaService = areaService;
            _deviceService = deviceService;
            _deviceTypeService = deviceTypeService; // Add this line
            _deviceFunctionService = deviceFunctionService; // Add this line
            _mapper = mapper;
        }

        public async Task<OverviewDto> GetDashboardOverview()
        {
            var areas = await _areaService.GetAreas();

            var Overview = new OverviewDto();

            foreach (var area in areas)
            {
                var devices = await _deviceService.GetDevicesByArea(area.Id);
                var OverviewArea = _mapper.Map<OverviewAreaDto>(area);
                foreach (var device in devices) // Modify this line
                {
                    var OverviewDevice = _mapper.Map<OverviewDeviceDto>(device);
                    var deviceType = await _deviceTypeService.GetDeviceType(device.DeviceTypeId); // Modify this line
                    if (deviceType == null)
                    {
                        throw new KeyNotFoundException("Device Type Not Found found");
                    }

                    var deviceFunctions = new List<DeviceFunctionDto>();

                    foreach (var deviceFunctionId in deviceType.Functions)
                    {
                        var deviceFunction = await _deviceFunctionService.GetDeviceFunction(deviceFunctionId);
                        if (deviceFunction == null)
                        {
                            throw new Exception("one or more Device Functions not found");
                        }
                        deviceFunctions.Add(deviceFunction);
                    }
                    OverviewDevice.DeviceType = deviceType;
                    OverviewDevice.DeviceFunctions = deviceFunctions;
                    OverviewArea.AreaDevices.Add(OverviewDevice);
                }
                if (!OverviewArea.AreaDevices.IsNullOrEmpty())
                {
                    Overview.Areas.Add(OverviewArea);
                }
            }

            return Overview;
        }
    }
}
