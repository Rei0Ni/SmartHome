using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using Serilog;
using SmartHome.Application.Interfaces;
using SmartHome.Application.Interfaces.Device;
using SmartHome.Dto.Sensors;

namespace SmartHome.Application.Services.Hosted
{
    public class MqttBackgroundService : BackgroundService
    {
        private readonly ILogger<MqttBackgroundService> _logger;
        private IMqttClient? _mqttClient;
        private readonly MqttClientOptions _mqttOptions;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public MqttBackgroundService(ILogger<MqttBackgroundService> logger, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            // --- Add Diagnostic Logging ---
            string? rawMqttHost = _configuration["MQTTHOST"]; // Read raw value
            bool fallbackUsed = rawMqttHost == null;
            string effectiveMqttHost = rawMqttHost ?? "localhost"; // Determine effective value

            _logger.LogInformation("MqttBackgroundService Constructor: MQTTHOST read from configuration: '{RawValue}', Fallback used: {FallbackUsed}", rawMqttHost, fallbackUsed);
            _logger.LogInformation("MqttBackgroundService Constructor: Using MQTT Broker Host: '{EffectiveMqttHost}' for connection options.", effectiveMqttHost);
            // --- End Diagnostic Logging ---

            var factory = new MqttClientFactory();
            _mqttClient = factory.CreateMqttClient();

            _mqttOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(effectiveMqttHost, 1883)
                .WithCredentials("mqttuser", "P@ssw0rd")
                .WithCleanSession()
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(60))
                .Build();
            _serviceProvider = serviceProvider;
            
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _mqttClient.ConnectedAsync += (async e =>
                {
                    _logger.LogInformation("Connected to MQTT Broker.");

                    // Subscribe to the sensor data topic
                    await _mqttClient.SubscribeAsync("sensor_data", MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
                    _logger.LogInformation("Subscribed to topic: sensor_data");
                });

                _mqttClient.DisconnectedAsync += (async e =>
                {
                    _logger.LogWarning("Disconnected from MQTT Broker. Reconnecting..." + e.Reason);
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    await _mqttClient.ConnectAsync(_mqttOptions, stoppingToken);
                });

                _mqttClient.ApplicationMessageReceivedAsync += (async e =>
                {
                    _logger.LogInformation("ApplicationMessageReceivedAsync event triggered!");

                    var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    _logger.LogInformation($"Received MQTT message on topic '{e.ApplicationMessage.Topic}': {payload}");

                    try
                    {
                        var sensorData = JsonSerializer.Deserialize<SensorDataDto>(payload);
                        if (sensorData != null)
                        {
                            //_logger.LogInformation($"{payload}");
                            // Create a new scope for each processing operation
                            using (var scope = _serviceProvider.CreateScope())
                            {
                                var deviceService = scope.ServiceProvider.GetRequiredService<IDeviceService>();
                                await deviceService.UpdateSensorDataAsync(sensorData);

                                //var dashboardService = scope.ServiceProvider.GetRequiredService<IDashboardService>();
                                //await dashboardService.SendOverviewUpdateToAll();  // Remove from here
                            }
                            _logger.LogInformation("Sensor data updated in database.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error processing MQTT message: {ex}");
                    }
                });

                await _mqttClient.ConnectAsync(_mqttOptions, stoppingToken);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error starting MQTT background service.");
                await StopAsync(stoppingToken);
            }

        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_mqttClient != null)
            {
                await _mqttClient.DisconnectAsync();
            }
        }
    }
}
