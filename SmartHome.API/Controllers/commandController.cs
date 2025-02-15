using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Application.Interfaces;
using SmartHome.Application.Interfaces.Controller;
using SmartHome.Dto.Command;

namespace SmartHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class commandController : ControllerBase
    {
        private readonly ICommandService _commandService;
        private readonly IControllerService _controllerService;
        public commandController(ICommandService commandService, IControllerService controllerService)
        {
            _commandService = commandService;
            _controllerService = controllerService;
        }

        [HttpPost("send-command")]
        public async Task<IActionResult> SendCommand(CommandRequestDto command)
        {
            var controllerIp = await _controllerService.GetControllerIpAsync(command.ControllerId);
            if (controllerIp == null)
                return NotFound("Controller not found");

            var response = await _commandService.SendCommandAsync(controllerIp, command);
            return Ok(response);
        }
    }
}
