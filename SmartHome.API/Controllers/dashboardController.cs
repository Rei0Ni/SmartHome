using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Application.Interfaces;

namespace SmartHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class dashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        public dashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("overview")]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _dashboardService.GetDashboardOverview();
            return Ok(result);
        }
    }
}
