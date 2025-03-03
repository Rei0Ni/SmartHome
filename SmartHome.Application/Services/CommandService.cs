using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Serilog;
using SmartHome.Application.Interfaces;
using SmartHome.Application.Interfaces.Device;
using SmartHome.Dto.Command;

namespace SmartHome.Application.Services
{
    public class CommandService : ICommandService
    {
        private readonly HttpClient _httpClient;
        private readonly IDeviceService _deviceService;

        public CommandService(IHttpClientFactory httpClientFactory, IDeviceService deviceService)
        {
            _httpClient = httpClientFactory.CreateClient("ControllerClient");
            _deviceService = deviceService;
        }

        public async Task<CommandResponseDto> SendCommandAsync(string controllerIp, CommandRequestDto commandRequest)
        {
            // Use options to convert property names to camelCase so that "AreaId" becomes "area"
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };

            // Serialize the command request DTO to JSON
            var json = JsonSerializer.Serialize(commandRequest, options);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var requestUri = $"http://{controllerIp}:2826/api/command/send";

            // Use PostAsync to send the request
            var response = await _httpClient.PostAsync(requestUri, content);

            // Throws an exception if the response indicates an error status.
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            var commandResponse = JsonSerializer.Deserialize<CommandResponseDto>(responseBody, options);

            // **--- Device State Update Logic using DeviceService ---**
            if (commandResponse?.Devices != null)
            {
                foreach (var deviceResponse in commandResponse.Devices)
                {
                    if (deviceResponse.Status == "success") // Process only on success
                    {
                        // **Delegate state update to DeviceService**
                        await _deviceService.UpdateDeviceStateFromResponseAsync(deviceResponse);
                    }
                    else
                    {
                        // Handle error status for this specific device if needed
                        Log.Error($"Command failed for device {deviceResponse.DeviceId}: {deviceResponse.Message}");
                        // ... error handling ...
                    }
                }
                commandResponse.Status = "Success";
            }
            else
            {
                Log.Error("No device responses found in CommandResponseDto.");
            }

            return commandResponse;
        }

    }
}
