namespace SmartHome.Dto.Weather
{
    using System.Text.Json.Serialization; // Use System.Text.Json for modern .NET

    public class MainData
    {
        [JsonPropertyName("temp")]
        public double Temp { get; set; } // Temperature

        [JsonPropertyName("feels_like")]
        public double FeelsLike { get; set; } // Temperature accounting for human perception of weather

        [JsonPropertyName("pressure")]
        public int Pressure { get; set; } // Atmospheric pressure on the sea level, hPa

        [JsonPropertyName("humidity")]
        public int Humidity { get; set; } // Humidity, %

        [JsonPropertyName("temp_min")]
        public double TempMin { get; set; } // Minimum temperature at the moment

        [JsonPropertyName("temp_max")]
        public double TempMax { get; set; } // Maximum temperature at the moment

        [JsonPropertyName("sea_level")]
        public int? SeaLevel { get; set; } // Atmospheric pressure on the sea level, hPa (Optional as it might not always be present)

        [JsonPropertyName("grnd_level")]
        public int? GrndLevel { get; set; } // Atmospheric pressure on the ground level, hPa (Optional)
    }
}
