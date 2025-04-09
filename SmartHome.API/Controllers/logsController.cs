using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Application.Interfaces.Logs;

namespace SmartHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class logsController : ControllerBase
    {
        private readonly ILogsService _logsService;

        public logsController(ILogsService logsService)
        {
            _logsService = logsService;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetLogs()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User not authenticated");
            }

            if (HttpContext.User.IsInRole("Admin"))
            {
                var logs = _logsService.GetLogsAsync(1, 25);
                if (logs == null)
                {
                    return NotFound("No logs found");
                }
                return Ok(logs);
            }
            else
            {
                var logs = _logsService.GetLogsByUserAsync(new Guid(userId), 1, 25);
                if (logs == null)
                {
                    return NotFound("No logs found for the logged in User");
                }
                return Ok(logs);
            }
        }
    }
}
