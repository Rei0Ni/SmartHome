using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SmartHome.Application.Interfaces.Area;
using SmartHome.Application.Interfaces.Controller;
using SmartHome.Application.Interfaces.Device;
using SmartHome.Application.Interfaces.ESPConfig;
using SmartHome.Dto.Controller;
using SmartHome.Dto.ESPConfig;
using SmartHome.Enum;

namespace SmartHome.Application.Services
{
    public class ESPConfigService : IESPConfigService
    {
        private readonly IControllerService _controllerService;
        private readonly IAreaService _areaService;
        private readonly IDeviceService _deviceService;
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public ESPConfigService(
            IControllerService controllerService,
            IAreaService areaService,
            IDeviceService deviceService,
            IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _controllerService = controllerService;
            _areaService = areaService;
            _deviceService = deviceService;
            _config = config;
            _httpClient = httpClientFactory.CreateClient("ESPConfigClient");
        }
        public async Task<bool> UpdateESPCotrollerConfig(Guid controllerId)
        {
            // Get the controller's IP address  
            var controller = await _controllerService.GetController(new GetControllerDto() { Id = controllerId });
            if (controller == null)
            {
                return false;
            }

            var areas = await _areaService.GetAllAreas();
            var devices = await _deviceService.GetDevices();

            var config = new ESPConfigDto()
            {
                Mqtt = new MqttConfigDto()
                {
                    server = _config["REMOTEMQTTHOST"],
                    publishInterval = 5000
                },
                Devices = devices
                    .Where(device => areas.Any(area => area.ControllerId == controllerId && area.Id == device.AreaId))
                    .Select(device => new DeviceConfigDto
                    {
                        areaId = device.AreaId.ToString(),
                        deviceId = device.Id.ToString(),
                        type = device.DeviceType.Type switch
                        {
                            // Map device type to corresponding PIN_TYPE  
                            var type when type == DeviceTypes.Lamp => "LED",
                            var type when type == DeviceTypes.FAN => "FAN",
                            var type when type == DeviceTypes.TEMPRATURE_SENSOR => "DHT11",
                            var type when type == DeviceTypes.PIR_MOTION_SENSOR => "PIR",
                            _ => throw new InvalidOperationException($"Unknown device type {device.DeviceTypeId}")
                        },
                        pin = device.Pin,
                        mode = device.DeviceType.Type switch
                        {
                            var type when type == DeviceTypes.Lamp => "OUTPUT",
                            var type when type == DeviceTypes.FAN => "OUTPUT",
                            var type when type == DeviceTypes.TEMPRATURE_SENSOR => "INPUT_PULLUP",
                            var type when type == DeviceTypes.PIR_MOTION_SENSOR => "INPUT",
                            _ => throw new InvalidOperationException($"Unknown device type {device.DeviceTypeId}")
                        }
                    }).ToList()
            };

            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Prepare the update config URL  
            var updateConfigUrl = $"http://{controller.IPAddress}:2826/api/config/update";

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Send the update config request  
            var updateResponse = await _httpClient.PostAsync(updateConfigUrl, content);

            var responseContent = await updateResponse.Content.ReadAsStringAsync();

            if (!updateResponse.IsSuccessStatusCode)
            {
                return false;
            }

            // Prepare the restart endpoint  
            var restartUrl = $"http://{controller.IPAddress}:2826/api/restart";

            // Send the restart request  
            var restartResponse = await _httpClient.GetAsync(restartUrl);

            if (restartResponse.IsSuccessStatusCode)
            {
                var controllerUpdateDto = new UpdateControllerDto()
                {
                    Id = controller.Id,
                    Name = controller.Name,
                    MACAddress = controller.MACAddress,
                    IPAddress = controller.IPAddress,
                    NeedsReconfiguration = false
                };
                await _controllerService.UpdateController(controllerUpdateDto);
            }

            return restartResponse.IsSuccessStatusCode;
        }
    }
}
