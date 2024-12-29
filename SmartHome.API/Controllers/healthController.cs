using Microsoft.AspNetCore.Mvc;
using Serilog;
using SmartHome.Application.Interfaces.Health;
using SmartHome.Application.Interfaces.MongoDBHealth.Service;
using SmartHome.Application.Services;
//using SmartHome.Application.Exceptions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class healthController : ControllerBase
    {
        private readonly IHealthCheck _healthCheck;

        public healthController(IHealthCheck healthCheck)
        {
            _healthCheck = healthCheck;
        }
        // GET: api/health
        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {
            //throw new AppException("API Malfunctioned");
            var systemHealth = await _healthCheck.CheckHealthAsync();
            return Ok(systemHealth);
        }
    }
}
