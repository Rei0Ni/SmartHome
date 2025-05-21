using System.Net.Sockets;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Application.Interfaces;
using SmartHome.Application.Interfaces.Controller;
using SmartHome.Application.Interfaces.Email;
using SmartHome.Application.Interfaces.Logs;
using SmartHome.Dto.Command;
using SmartHome.Dto.Controller;

namespace SmartHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class commandController : ControllerBase
    {
        private readonly ICommandService _commandService;
        private readonly IControllerService _controllerService;
        private readonly ILogsService _logsService;
        private readonly IEmailService _emailService;
        public commandController(ICommandService commandService, IControllerService controllerService, ILogsService logsService, IEmailService emailService)
        {
            _commandService = commandService;
            _controllerService = controllerService;
            _logsService = logsService;
            _emailService = emailService;
        }

        [HttpPost("send-command")]
        [Authorize]
        public async Task<IActionResult> SendCommand(CommandRequestDto command)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var controllerIp = await _controllerService.GetControllerIpAsync(command.ControllerId);
            if (controllerIp == null)
                return NotFound(new { message = "Controller not found" });

            try
            {
                var response = await _commandService.SendCommandAsync(controllerIp, command);

                await _logsService.AddLogAsync(response.Devices[0].Message, command.Devices[0].DeviceId, new Guid(userId!), command.AreaId, Enum.LogLevel.Info);

                return Ok(new { status = "success", data = response });
            }
            catch (SocketException socketException)
            {
                await _logsService.AddLogAsync("Controller is down or not responding", command.Devices[0].DeviceId, new Guid(userId!), command.AreaId, Enum.LogLevel.Error);

                var subject = "Controller Down Alert";
                var htmlContent = $"<p>The controller is down or not responding. Please check the system.</p><br><p>{socketException.Message}</p>";
                await _emailService.SendEmailToAdminsAsync(Enum.LogLevel.Error, subject, htmlContent);

                return StatusCode(StatusCodes.Status503ServiceUnavailable, new { message = "Controller is down or not responding" });
            }
            catch (HttpRequestException requestException) when (requestException.InnerException is SocketException)
            {
                await _logsService.AddLogAsync("Controller is down or not responding", command.Devices[0].DeviceId, new Guid(userId!), command.AreaId, Enum.LogLevel.Error);

                var subject = "Controller Down Alert";
                var htmlContent = $"<p>The controller is down or not responding. Please check the system.</p><br><p>{requestException.Message}</p>";
                await _emailService.SendEmailToAdminsAsync(Enum.LogLevel.Error, subject, htmlContent);

                return StatusCode(StatusCodes.Status504GatewayTimeout, new { message = "Controller did not respond in time" });
            }
            catch (TaskCanceledException cancelException)
            {
                await _logsService.AddLogAsync("Controller is down or not responding, The request timed out.", command.Devices[0].DeviceId, new Guid(userId!), command.AreaId, Enum.LogLevel.Error);

                var subject = "Controller Down Alert";
                var htmlContent = $"<p>The controller is down or not responding. Please check the system.</p><br><p>{cancelException.Message}</p>";
                await _emailService.SendEmailToAdminsAsync(Enum.LogLevel.Error, subject, htmlContent);

                return StatusCode(StatusCodes.Status408RequestTimeout, new { message = "The request timed out" });
            }
        }
    }
}
