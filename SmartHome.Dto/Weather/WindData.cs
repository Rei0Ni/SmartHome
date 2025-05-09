namespace SmartHome.Dto.Weather
{
    using System.Text.Json.Serialization; // Use System.Text.Json for modern .NET

    public class WindData
    {
        [JsonPropertyName("speed")]
        public double Speed { get; set; } // Wind speed

        [JsonPropertyName("deg")]
        public int Deg { get; set; } // Wind direction, degrees (meteorological)

        [JsonPropertyName("gust")]
        public double? Gust { get; set; } // Wind gust (Optional as it might not always be present)
    }
}
