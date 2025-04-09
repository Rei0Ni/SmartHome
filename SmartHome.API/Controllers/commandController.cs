using System.Net.Sockets;
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

            try
            {
                var response = await _commandService.SendCommandAsync(controllerIp, command);
                return Ok(response);
            }
            catch (SocketException)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Controller is down or not responding");
            }
            catch (HttpRequestException ex) when (ex.InnerException is SocketException)
            {
                return StatusCode(StatusCodes.Status504GatewayTimeout, "Controller did not respond in time");
            }
            catch (TaskCanceledException)
            {
                return StatusCode(StatusCodes.Status408RequestTimeout, "The request timed out");
            }
        }
    }
}
