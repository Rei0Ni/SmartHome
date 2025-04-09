using System.Net.Sockets;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Application.Interfaces;
using SmartHome.Application.Interfaces.Controller;
using SmartHome.Application.Interfaces.Logs;
using SmartHome.Dto.Command;

namespace SmartHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class commandController : ControllerBase
    {
        private readonly ICommandService _commandService;
        private readonly IControllerService _controllerService;
        private readonly ILogsService _logsService;
        public commandController(ICommandService commandService, IControllerService controllerService, ILogsService logsService)
        {
            _commandService = commandService;
            _controllerService = controllerService;
            _logsService = logsService;
        }

        [HttpPost("send-command")]
        [Authorize]
        public async Task<IActionResult> SendCommand(CommandRequestDto command)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var controllerIp = await _controllerService.GetControllerIpAsync(command.ControllerId);
            if (controllerIp == null)
                return NotFound("Controller not found");

            try
            {
                var response = await _commandService.SendCommandAsync(controllerIp, command);

                await _logsService.AddLogAsync(response.Devices[0].Message, command.Devices[0].DeviceId, new Guid(userId!), command.AreaId, Enum.LogLevel.Info);

                return Ok(response);
            }
            catch (SocketException)
            {
                await _logsService.AddLogAsync("Controller is down or not responding", command.Devices[0].DeviceId, new Guid(userId!), command.AreaId, Enum.LogLevel.Critical);

                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Controller is down or not responding");
            }
            catch (HttpRequestException ex) when (ex.InnerException is SocketException)
            {
                await _logsService.AddLogAsync("Controller is down or not responding", command.Devices[0].DeviceId, new Guid(userId!), command.AreaId, Enum.LogLevel.Critical);

                return StatusCode(StatusCodes.Status504GatewayTimeout, "Controller did not respond in time");
            }
            catch (TaskCanceledException)
            {
                await _logsService.AddLogAsync("Controller is down or not responding, The request timed out.", command.Devices[0].DeviceId, new Guid(userId!), command.AreaId, Enum.LogLevel.Critical);

                return StatusCode(StatusCodes.Status408RequestTimeout, "The request timed out");
            }
        }
    }
}
