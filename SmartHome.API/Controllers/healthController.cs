using Microsoft.AspNetCore.Mvc;
using Serilog;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class healthController : ControllerBase
    {
        // GET: api/health
        [HttpGet]
        public ActionResult Get()
        {
            Log.Information("API is Responding");
            return Ok(new { Status = 200, Message = "API is Responding"});
        }
    }
}
