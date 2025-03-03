using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartHome.Dto.Sensors
{
    public class SensorAreaDto
    {
        [JsonPropertyName("areaId")]
        public string AreaId { get; set; }

        [JsonPropertyName("sensors")]
        public List<SensorDto> Sensors { get; set; } = new();
    }
}
