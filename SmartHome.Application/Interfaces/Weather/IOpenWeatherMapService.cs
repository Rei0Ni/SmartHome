using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Dto.Weather;

namespace SmartHome.Application.Interfaces.Weather
{
    public interface IOpenWeatherMapService
    {
        Task<WeatherResponse> GetCurrentWeatherJsonAsync();
        Task<string> GetCityKey();
    }
}
