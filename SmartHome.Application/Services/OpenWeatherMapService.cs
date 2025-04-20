using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SmartHome.Application.Interfaces.Weather;
using SmartHome.Dto.Weather;

namespace SmartHome.Application.Services
{
    public class OpenWeatherMapService : IOpenWeatherMapService
    {
        private readonly string _apiKey;
        private readonly string _cityKey;
        private readonly string _countryKey;
        private readonly string _units;
        private readonly HttpClient _httpClient;


        public OpenWeatherMapService(IConfiguration _config, IHttpClientFactory httpClientFactory)
        {
            _apiKey = _config["Weather:APIKey"] ?? "68394ea6a79eda06b9a812e1c87ac657";
            _cityKey = _config["Weather:City"] ?? "Cairo";
            _countryKey = _config["Weather:Country"] ?? "EG";
            _units = _config["Weather:Units"] ?? "metric";
            _httpClient = httpClientFactory.CreateClient("WeatherClient");
        }

        public Task<string> GetCityKey()
        {
            return Task.FromResult($"{_cityKey},{_countryKey}");
        }

        public async Task<WeatherResponse> GetCurrentWeatherJsonAsync()
        {
            var requestUrl = $"?q={_cityKey},{_countryKey}&units={_units}&appid={_apiKey}";

            var response = await _httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode(); // Throws an exception for non-success status codes

            var WeatherResponse = await response.Content.ReadFromJsonAsync<WeatherResponse>();

            return WeatherResponse!;
        }
    }
}
