using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Serilog;
using SmartHome.Application.Interfaces;
using SmartHome.Dto.Command;

namespace SmartHome.Application.Services
{
    public class CommandService : ICommandService
    {
        private readonly HttpClient _httpClient;

        public CommandService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ControllerClient");
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

            Log.Information("Sending command to {RequestUri}: {JsonContent}", requestUri, json);

            // Use PostAsync to send the request
            var response = await _httpClient.PostAsync(requestUri, content);
            Log.Information("Response: {@Response}", response);

            // Throws an exception if the response indicates an error status.
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            Log.Information("Response Body: {ResponseBody}", responseBody);

            var commandResponse = JsonSerializer.Deserialize<CommandResponseDto>(responseBody, options);

            return commandResponse;
        }

    }
}
