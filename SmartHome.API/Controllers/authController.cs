using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Dto;
using SmartHome.Dto.User;
using SmartHome.Application.Interfaces.User;
using SmartHome.Application.Services.User;
using SmartHome.Enum;

namespace SmartHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class authController : ControllerBase
    {
        private readonly IUserService _userService;

        public authController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<ActionResult> LoginAsync(LoginDto dto)
        {
            var response = await _userService.LoginAsync(dto);
            return Ok(response);
        }

        [HttpGet("authentication-state")]
        [Authorize]
        public async Task<UserAuthenticationState> AuthenticationState()
        {
            var user = await _userService.GetUserAuthenticationStateAsync(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? String.Empty) ?? new UserAuthenticationState();
            user.IsAuthenticated = HttpContext.User.Identity!.IsAuthenticated;
            // Keys to exclude
            var excludedKeys = new HashSet<string> { "jti", "exp", "iss", "aud" };
            user.Claims = HttpContext.User.Claims
                .Where(c => !excludedKeys.Contains(c.Type))
                .ToDictionary(c => c.Type, c => c.Value);
            return user;
        }

        [HttpGet("test_auth")]
        [Authorize]
        public ActionResult TestAuthentication()
        {
            return Ok(new ApiResponse<object>()
            {
                Status = ApiResponseStatus.Success.ToString(),
                Message = "it's working"
            });
        }
    }
}
