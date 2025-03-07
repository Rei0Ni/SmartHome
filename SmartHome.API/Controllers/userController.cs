using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Application.Interfaces.User;
using SmartHome.Dto;
using SmartHome.Dto.User;
using SmartHome.Enum;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class userController : ControllerBase
    {
        private readonly IUserService _userService;

        public userController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("create/admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> CreateAdminUser(RegisterAdminUserDto dto)
        {
            var response = await _userService.CreateAdminUserAsync(dto);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("create/user")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> CreateNormalUser(RegisterUserDto dto)
        {
            var response = await _userService.CreateNormalUserAsync(dto);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("create/guest")]
        [Authorize(Roles = "Admin,Normal_User")]
        public async Task<ActionResult<ApiResponse<object>>> CreateGuestUser(RegisterUserDto dto)
        {
            var response = await _userService.CreateGuestUserAsync(dto);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> CreateUser(RegisterUserDto dto, Role role)
        {
            var response = await _userService.CreateUserAsync(dto, role);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        // PUT api/<userController>/5
        [HttpPut("update")]
        [Authorize(Roles = "Admin,Normal_User")]
        public async Task<ActionResult<ApiResponse<object>>> Update(UpdateUserDto dto)
        {
            var id = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id != dto.UserId)
            {
                return BadRequest("User ID mismatch");
            }

            var response = await _userService.UpdateUserProfileAsync(dto);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        // DELETE api/<userController>/5
        [HttpDelete("delete")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(DeleteUserDto dto)
        {
            var response = await _userService.DeleteUserAsync(dto.Id);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
