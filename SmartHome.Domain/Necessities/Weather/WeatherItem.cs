namespace SmartHome.Domain.Necessities.Weather
{
    using System.Text.Json.Serialization; // Use System.Text.Json for modern .NET

    public class WeatherItem
    {
        [JsonPropertyName("id")]
        public int Id { get; set; } // Weather condition id

        [JsonPropertyName("main")]
        public string Main { get; set; } // Group of weather parameters (Rain, Snow, Clouds etc.)

        [JsonPropertyName("description")]
        public string Description { get; set; } // Weather condition within the group

        [JsonPropertyName("icon")]
        public string Icon { get; set; } // Weather icon id
    }
}
