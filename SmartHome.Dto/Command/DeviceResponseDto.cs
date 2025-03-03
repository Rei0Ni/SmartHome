using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartHome.Dto.Command
{
    public class DeviceResponseDto
    {
        public Guid DeviceId { get; set; }
        public string Status { get; set; } // "success" or "error" for this specific device command
        public string Message { get; set; } // Message for this device command

        [JsonPropertyName("power_state")]
        public string PowerState { get; set; }

        [JsonPropertyName("fan_speed")]
        public int? FanSpeed { get; set; }
    }
}
