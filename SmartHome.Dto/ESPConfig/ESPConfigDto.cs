using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartHome.Dto.ESPConfig
{

    public class ESPConfigDto
    {
        [JsonPropertyName("mqtt")]
        public MqttConfigDto Mqtt { get; set; } = new MqttConfigDto();
        [JsonPropertyName("devices")]
        public List<DeviceConfigDto> Devices { get; set; } = new List<DeviceConfigDto>();
    }
}
