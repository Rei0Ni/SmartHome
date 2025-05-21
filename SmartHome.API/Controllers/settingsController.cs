using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Application.Interfaces.Settings;
using SmartHome.Dto;
using SmartHome.Dto.Email;
using SmartHome.Dto.Settings;

namespace SmartHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class settingsController : ControllerBase
    {
        private readonly ISettingsService _settingsService;

        public settingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [HttpGet("get/all")]
        public async Task<IActionResult> GetAllSettings()
        {
            var settings = await _settingsService.GetSettingsAsync();
            return Ok(settings);
        }

        [HttpGet("get/email")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> GetEmailSettings()
        {
            var emailSettings = await _settingsService.GetEmailSettingsAsync();
            return Ok(emailSettings);
        }

        [HttpPost("update")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> UpdateSetting([FromBody] SaveSettingsDto settingsDto)
        {
            if (settingsDto == null || settingsDto.Settings == null || !settingsDto.Settings.Any())
                return BadRequest("Invalid settings data.");

            foreach (var setting in settingsDto.Settings)
            {
                if (string.IsNullOrEmpty(setting.Key))
                    return BadRequest("Each setting must have a valid key.");

                var result = await _settingsService.UpdateSettingsAsync(setting);
                if (!result)
                    return StatusCode(500, $"Failed to save setting with key: {setting.Key}");
            }

            return Ok("Settings saved successfully.");
        }

        [HttpPut("email/update")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> UpdateEmailSettings([FromBody] UpdateEmailSettingsDto emailSettings)
        {
            if (emailSettings == null)
                return BadRequest("Invalid email settings data.");

            await _settingsService.UpdateEmailSettingsAsync(emailSettings);
            return Ok("Email settings updated successfully.");
        }

        [HttpGet("exists/{key}")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<ApiResponse<object>>> SettingsExists(string key)
        {
            if (string.IsNullOrEmpty(key))
                return BadRequest("Key cannot be null or empty.");

            var exists = await _settingsService.SettingsExistsAsync(key);

            var response = new ApiResponse<object>() // Fixed type to match ApiResponse<object>  
            {
                Status = exists ? "success" : "failure",
                Message = exists ? $"The Setting with Key '{key}' Exists" : $"The Setting with Key '{key}' Doesn't Exist",
                Data = new { Exists = exists }  
            };
            return Ok(response); // Return the response object  
        }
    }
}
