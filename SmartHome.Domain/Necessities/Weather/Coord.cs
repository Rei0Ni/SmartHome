namespace SmartHome.Domain.Necessities.Weather
{
    using System.Text.Json.Serialization; // Use System.Text.Json for modern .NET

    public class Coord
    {
        [JsonPropertyName("lon")]
        public double Lon { get; set; } // Longitude of the location

        [JsonPropertyName("lat")]
        public double Lat { get; set; } // Latitude of the location
    }
}
