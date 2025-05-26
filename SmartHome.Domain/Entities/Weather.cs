using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using SmartHome.Domain.Necessities.Weather;

namespace SmartHome.Domain.Entities
{
    public class Weather
    {
        public Guid Id { get; set; }

        // Optional: Keep if you want to know *which* City's weather is cached
        public string CityKey { get; set; }

        // The raw JSON response from the OpenWeatherMap API
        public WeatherResponse WeatherDataJson { get; set; }

        // Timestamp when the data was cached
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
