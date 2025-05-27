using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using DnsClient.Internal;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Serilog;
using SmartHome.Application.Hubs;
using SmartHome.Application.Interfaces;
using SmartHome.Application.Interfaces.Area;
using SmartHome.Application.Interfaces.Controller;
using SmartHome.Application.Interfaces.Device;
using SmartHome.Application.Interfaces.DeviceType;
using SmartHome.Application.Interfaces.Hubs;
using SmartHome.Domain.Entities;
using SmartHome.Dto.Command;
using SmartHome.Dto.Device;
using SmartHome.Dto.DeviceType;
using SmartHome.Dto.Sensors;
using SmartHome.Enum;
using Log = Serilog.Log;

namespace SmartHome.Application.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IDeviceTypeService _deviceTypeService;
        private readonly IDeviceTypeRepository _deviceTypeRepository;
        //private readonly IDeviceFunctionService _deviceFunctionService;
        private readonly IAreaRepository _areaRepository;
        private readonly IControllerRepository _controllerRepository;
        private readonly IHubContext<OverviewHub> _overviewHubContext;
        private readonly IHubState _hubState;
        private IValidator<CreateDeviceDto> _createDeviceDtoValidator;
        private IValidator<UpdateDeviceDto> _updateDeviceDtoValidator;
        private IValidator<DeleteDeviceDto> _deleteDeviceDtoValidator;
        private IMapper _mapper;
        private readonly IDashboardService _dashboardService;
        private readonly IDeviceDataService _deviceDataService; // Inject the new service

        public DeviceService(
            IDeviceRepository deviceRepository,
            IValidator<CreateDeviceDto> createDeviceDtoValidator,
            IValidator<UpdateDeviceDto> updateDeviceDtoValidator,
            IValidator<DeleteDeviceDto> deleteDeviceDtoValidator,
            IMapper mapper,
            IDeviceTypeService deviceTypeService,
            //IDeviceFunctionService deviceFunctionService,
            IDeviceTypeRepository deviceTypeRepository,
            IAreaRepository areaRepository,
            IHubContext<OverviewHub> overviewHubContext,
            IHubState hubState,
            IDashboardService dashboardService,
            IDeviceDataService deviceDataService,
            IControllerRepository controllerRepository) // Add IDeviceDataService to the constructor
        {
            _deviceRepository = deviceRepository;
            _createDeviceDtoValidator = createDeviceDtoValidator;
            _updateDeviceDtoValidator = updateDeviceDtoValidator;
            _deleteDeviceDtoValidator = deleteDeviceDtoValidator;
            _mapper = mapper;
            _deviceTypeService = deviceTypeService;
            //_deviceFunctionService = deviceFunctionService;
            _deviceTypeRepository = deviceTypeRepository;
            _areaRepository = areaRepository;
            _overviewHubContext = overviewHubContext;
            _hubState = hubState;
            _dashboardService = dashboardService;
            _deviceDataService = deviceDataService; // Store the injected instance
            _controllerRepository = controllerRepository;
        }


        public async Task CreateDevice(CreateDeviceDto createDeviceDto)
        {
            var validationResult = await _createDeviceDtoValidator.ValidateAsync(createDeviceDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var device = _mapper.Map<Device>(createDeviceDto);

            // Initialize basic state based on the device type  
            var deviceType = await _deviceTypeRepository.GetDeviceType(device.DeviceTypeId);
            if (deviceType != null)
            {
                device.State = new Dictionary<string, object>();

                switch (deviceType.Type)
                {
                    case DeviceTypes.Lamp:
                        device.State["power_state"] = "off";
                        device.State["brightness"] = 0;
                        break;
                    case DeviceTypes.TEMPRATURE_SENSOR:
                        device.State["temperature_celsius"] = 0.0;
                        device.State["humidity_percent"] = 0.0;
                        break;
                    case DeviceTypes.FAN:
                        device.State["power_state"] = "off";
                        device.State["fan_speed"] = 0;
                        break;
                    case DeviceTypes.PIR_MOTION_SENSOR:
                        device.State["motion_detected"] = false;
                        break;
                    default:
                        Log.Warning($"No default state defined for device type {deviceType.Type}");
                        break;
                }
            }

            await _deviceRepository.CreateDevice(device);

            deviceType.Devices.Add(device.Id);
            await _deviceTypeRepository.UpdateDeviceType(deviceType);

            var area = await _areaRepository.GetArea(device.AreaId);
            var controller = await _controllerRepository.GetController(area.ControllerId);
            area.Devices.Add(device.Id);
            controller.NeedsReconfiguration = true;
            await _areaRepository.UpdateArea(area);
            await _controllerRepository.UpdateController(controller);
        }

        public async Task DeleteDevice(DeleteDeviceDto deleteDeviceDto)
        {
            var validationResult = await _deleteDeviceDtoValidator.ValidateAsync(deleteDeviceDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
            var device = _mapper.Map<Device>(await _deviceRepository.GetDevice(deleteDeviceDto.Id));

            if (device == null)
            {
                throw new KeyNotFoundException("Device not found");
            }
            var area = await _areaRepository.GetArea(device.AreaId);
            var controller = await _controllerRepository.GetController(area.ControllerId);
            area.Devices.Remove(device.Id);
            controller.NeedsReconfiguration = true;
            await _areaRepository.UpdateArea(area);
            await _controllerRepository.UpdateController(controller);
            await _deviceRepository.DeleteDevice(device);
        }

        public async Task<DeviceDto> GetDevice(Guid id)
        {
            var device = await _deviceRepository.GetDevice(id);
            if (device == null)
            {
                throw new KeyNotFoundException("Device not found");
            }



            var deviceType = await _deviceTypeService.GetDeviceType(device.DeviceTypeId);
            if (deviceType == null)
            {
                throw new KeyNotFoundException("Device Type Not Found found");
            }

            var dto = _mapper.Map<DeviceDto>(device);
            dto.DeviceType = deviceType;

            return dto;
        }

        public async Task<List<DeviceDto>> GetDevices()
        {
            var devices = await _deviceRepository.GetDevices();
            var deviceDtos = _mapper.Map<List<DeviceDto>>(devices);
            var deviceTypes = await _deviceTypeRepository.GetDeviceTypes();

            // Map device types to their corresponding deviceDtos  
            foreach (var deviceDto in deviceDtos)
            {
                var deviceType = deviceTypes.FirstOrDefault(dt => dt.Id == deviceDto.DeviceTypeId);
                if (deviceType != null)
                {
                    deviceDto.DeviceType = _mapper.Map<DeviceTypeDto>(deviceType);
                }
            }

            return deviceDtos;
        }

        public async Task<List<DeviceDto>> GetDevicesByArea(Guid Id)
        {
            var devices = await _deviceRepository.GetDevicesByArea(Id);
            return _mapper.Map<List<DeviceDto>>(devices);
        }

        public async Task<List<DeviceDto>> GetDevicesForAreas(List<Guid> areaIds)
        {
            // var devices = await _deviceRepository.GetDevicesForAreas(areaIds); //No longer use DeviceRepository
            var devices = await _deviceDataService.GetDevicesForAreas(areaIds);
            return _mapper.Map<List<DeviceDto>>(devices);
        }

        public async Task UpdateDevice(UpdateDeviceDto updateDeviceDto)
        {
            var validationResult = await _updateDeviceDtoValidator.ValidateAsync(updateDeviceDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var device = await _deviceRepository.GetDevice(updateDeviceDto.Id);
            if (device == null)
            {
                throw new KeyNotFoundException("Device not found");
            }

            if(device.Pin != updateDeviceDto.Pin)
            {
                var area = await _areaRepository.GetArea(device.AreaId);
                var controller = await _controllerRepository.GetController(area.ControllerId);
                controller.NeedsReconfiguration = true;
                await _controllerRepository.UpdateController(controller);
            }

            _mapper.Map(updateDeviceDto, device);
            await _deviceRepository.UpdateDevice(device);
        }

        public async Task UpdateDeviceStateFromResponseAsync(DeviceResponseDto deviceResponse)
        {
            var device = await _deviceRepository.GetDevice(deviceResponse.DeviceId); // Get Device 

            if (device != null)
            {
                if (deviceResponse.Status == "success")
                {
                    // Update State Dictionary based on successful response
                    if (deviceResponse.PowerState != null)
                    {
                        device.State["power_state"] = deviceResponse.PowerState;
                    }

                    if (deviceResponse.Brightness.HasValue)
                    {
                        device.State["brightness"] = deviceResponse.Brightness.Value;
                    }

                    if (deviceResponse.FanSpeed.HasValue)
                    {
                        device.State["fan_speed"] = deviceResponse.FanSpeed.Value;
                    }

                    device.LastUpdated = DateTime.UtcNow; // Update LastUpdated timestamp
                    await _deviceRepository.UpdateDevice(device); // Save 

                    // Notify connected users of the change
                    await SendOverviewUpdateToAllConnectedUsers(); // Call the new method

                }
                else
                {
                    // Log 
                    Log.Warning($"Command failed for device {deviceResponse.DeviceId}: {deviceResponse.Message}");
                }
            }
            else
            {
                // Log
                Log.Warning($"Device with ID {deviceResponse.DeviceId} not found in repository during state update.");
            }
        }

        public async Task UpdateSensorDataAsync(SensorDataDto sensorData)
        {
            if (sensorData == null)
            {
                throw new ArgumentNullException(nameof(sensorData));
            }

            if (!string.Equals(sensorData.Status, "success", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception($"Sensor data response returned an error: {sensorData.Message}");
            }

            // Iterate through each area in the response
            foreach (var area in sensorData.Areas)
            {
                // Process each sensor in the area
                foreach (var sensor in area.Sensors)
                {
                    // If your device IDs are stored as Guid, attempt to parse:
                    if (Guid.TryParse(sensor.DeviceId, out Guid deviceId))
                    {
                        var device = await _deviceRepository.GetDevice(deviceId);
                        if (device != null)
                        {
                            // Update the device state with sensor values
                            // Here, assuming device.State is a dictionary<string, object>

                            if (sensor.TemperatureCelsius != default)
                            {
                                device.State["temperature_celsius"] = sensor.TemperatureCelsius;
                            }

                            if (sensor.HumidityPercent != default)
                            {
                                device.State["humidity_percent"] = sensor.HumidityPercent;
                            }

                            if (sensor.MotionDetected != null)
                            {
                                device.State["motion_detected"] = sensor.MotionDetected;
                            }

                            if (sensor.MotionDetected != null)
                            {
                                device.State["motion_detected"] = sensor.MotionDetected;
                            }

                            if (sensor.MotionDetected != null)
                            {
                                device.State["motion_detected"] = sensor.MotionDetected;
                            }

                            //// Optionally update other sensor properties
                            //if (!string.IsNullOrEmpty(sensor.Status))
                            //{
                            //    device.State["sensor_status"] = sensor.Status;
                            //}
                            //if (!string.IsNullOrEmpty(sensor.Type))
                            //{
                            //    device.State["sensor_type"] = sensor.Type;
                            //}

                            device.LastUpdated = DateTime.UtcNow; // Update timestamp
                            // Save the updated device
                            await _deviceRepository.UpdateDevice(device);

                            // Notify connected users of the change
                            await SendOverviewUpdateToAllConnectedUsers(); // Call the new method

                        }
                        else
                        {
                            // Handle case where device is not found (e.g., log a warning)
                            Log.Warning($"Device with ID {sensor.DeviceId} not found.");
                        }
                    }
                    else
                    {
                        // If DeviceId is not a Guid, you might have alternative lookup logic
                        Log.Warning($"Unable to parse DeviceId '{sensor.DeviceId}' as a Guid.");
                    }
                }
            }
        }

        private async Task SendOverviewUpdateToAllConnectedUsers()
        {
            var connectedUsers = _hubState.GetConnectedUsers();
            foreach (var userId in connectedUsers)
            {
                try
                {
                    // Use the injected DashboardService to get the overview data
                    var overviewData = await _dashboardService.GetDashboardOverview(userId);
                    await _overviewHubContext.Clients.Group(userId).SendAsync("ReceiveOverviewData", overviewData);
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to send overview update to user {userId}: {ex.Message}");
                }
            }
        }
    }
}