using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Serilog;
using SmartHome.Application.Interfaces.Weather;
using SmartHome.Domain.Entities;
using SmartHome.Dto.Weather;
using Log = Serilog.Log;

namespace SmartHome.Application.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IOpenWeatherMapService _openWeatherMapService;
        private readonly IWeatherRepository _weatherRepository;
        public IMapper _mapper;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(15);

        public WeatherService(IOpenWeatherMapService openWeatherMapService, IWeatherRepository weatherRepository, IMapper mapper)
        {
            _openWeatherMapService = openWeatherMapService;
            _weatherRepository = weatherRepository;
            _mapper = mapper;
        }
        public async Task<WeatherResponse> GetWeatherAsync()
        {
            // 1. Check Cache (the single cached item)
            var Weather = await _weatherRepository.GetLatestWeatherAsync();
            var currentTime = DateTimeOffset.UtcNow;

            bool isCacheValid = false;
            if (Weather != null)
            {
                // You might also want to check if the cached location matches the requested locationIdentifier
                // depending on your exact needs for a single cache object.
                // For a true "single latest" cache, you might skip location check here.
                if ((currentTime - Weather.Timestamp) <= _cacheDuration)
                {
                    Log.Information($"Serving cached weather (latest fetched)");
                    isCacheValid = true;
                }
                else
                {
                    Log.Information($"Cached weather expired (latest fetched)");
                }
            }
            else
            {
                Log.Information($"No weather cached yet.");
            }


            if (isCacheValid)
            {
                var WeatherResponse = _mapper.Map<WeatherResponse>(Weather!.WeatherDataJson);
                return WeatherResponse;
            }


            // 2. Fetch from API
            Log.Information($"Fetching new weather data");
            var CityKey = await _openWeatherMapService.GetCityKey();
            try
            {
                // Your IOpenWeatherMapService needs to handle fetching by different identifiers
                var weatherData = await _openWeatherMapService.GetCurrentWeatherJsonAsync();
                var WeatherResponse = _mapper.Map<SmartHome.Domain.Necessities.Weather.WeatherResponse>(weatherData);

                // 3. Save to Cache (overwriting the single cached item)
                var newWeather = new Weather
                {
                    CityKey = CityKey,
                    WeatherDataJson = WeatherResponse,
                    Timestamp = DateTime.UtcNow
                };
                await _weatherRepository.SaveOrUpdateWeatherAsync(newWeather);

                Log.Information($"Fetched and cached new weather for {CityKey}");
                return weatherData;
            }
            catch (HttpRequestException ex)
            {
                Log.Error($"API error fetching weather for {CityKey}: {ex.Message}");
                // Handle API error - you could return the expired cached data here as a fallback
                if (Weather != null)
                {
                    Log.Error($"API error, serving expired cache for {CityKey} as fallback");
                    return _mapper.Map<WeatherResponse>(Weather!.WeatherDataJson);
                }
                throw; // Re-throw if no cache is available
            }
        }
    }
}
