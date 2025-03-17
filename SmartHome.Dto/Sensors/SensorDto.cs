using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartHome.Dto.Sensors
{
    public class SensorDto
    {
        [JsonPropertyName("deviceId")]
        public string DeviceId { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("temperature_celsius")]
        public double TemperatureCelsius { get; set; }

        [JsonPropertyName("humidity_percent")]
        public int HumidityPercent { get; set; }

        [JsonPropertyName("motion_detected")]
        public bool? MotionDetected { get; set; } = null;
    }
}
