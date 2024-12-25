using Microsoft.AspNetCore.Mvc;
using Serilog;
using SmartHome.Application.Interfaces.MongoDBHealth.Service;
//using SmartHome.Application.Exceptions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class healthController : ControllerBase
    {
        public readonly IMongoDBHealth MongoDBHealth;

        public healthController(IMongoDBHealth mongoDBHealth)
        {
            MongoDBHealth = mongoDBHealth;
        }
        // GET: api/health
        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {
            Log.Information("API is Responding");
            //throw new AppException("API Malfunctioned");
            var DBHealth = await MongoDBHealth.Ping();
            return Ok(new { status = 200, message = "API is Responding", db_health = DBHealth});
        }
    }
}
