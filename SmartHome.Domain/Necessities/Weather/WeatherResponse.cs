using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartHome.Domain.Necessities.Weather
{
    public class WeatherResponse
    {
        [JsonPropertyName("coord")]
        public Coord Coord { get; set; }

        [JsonPropertyName("weather")]
        public List<WeatherItem> Weather { get; set; }

        [JsonPropertyName("base")]
        public string Base { get; set; }

        [JsonPropertyName("main")]
        public MainData Main { get; set; }

        [JsonPropertyName("visibility")]
        public int Visibility { get; set; } // Max 10000 meters (10 km)

        [JsonPropertyName("wind")]
        public WindData Wind { get; set; }

        [JsonPropertyName("clouds")]
        public CloudsData Clouds { get; set; }

        [JsonPropertyName("dt")]
        public long Dt { get; set; } // Time of data calculation, unix, UTC

        [JsonPropertyName("sys")]
        public SystemData Sys { get; set; }

        [JsonPropertyName("timezone")]
        public int Timezone { get; set; } // Shift in seconds from UTC

        [JsonPropertyName("id")]
        public int Id { get; set; } // City ID

        [JsonPropertyName("name")]
        public string Name { get; set; } // City name

        [JsonPropertyName("cod")]
        public int Cod { get; set; } // Internal parameter (often the HTTP status code)
    }
}
