using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SmartHome.Application.Interfaces.Weather;

namespace SmartHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class weatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public weatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetWeather()
        {
            var WeatherResponse = await _weatherService.GetWeatherAsync();
            return Ok(WeatherResponse);
        }
    }
}
