namespace SmartHome.Dto.Weather
{
    using System.Text.Json.Serialization; // Use System.Text.Json for modern .NET

    public class SystemData
    {
        [JsonPropertyName("type")]
        public int? Type { get; set; } // Internal parameter (Optional)

        [JsonPropertyName("id")]
        public int? Id { get; set; } // Internal parameter (Optional)

        [JsonPropertyName("message")]
        public string Message { get; set; } // Internal parameter (Deprecated/Optional)

        [JsonPropertyName("country")]
        public string Country { get; set; } // Country code (GB, JP etc.)

        [JsonPropertyName("sunrise")]
        public long Sunrise { get; set; } // Sunrise time, unix, UTC

        [JsonPropertyName("sunset")]
        public long Sunset { get; set; } // Sunset time, unix, UTC
    }
}
