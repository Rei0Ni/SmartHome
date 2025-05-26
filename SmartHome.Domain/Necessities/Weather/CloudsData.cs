namespace SmartHome.Domain.Necessities.Weather
{
    using System.Text.Json.Serialization; // Use System.Text.Json for modern .NET

    public class CloudsData
    {
        [JsonPropertyName("all")]
        public int All { get; set; } // Cloudiness, %
    }
}
