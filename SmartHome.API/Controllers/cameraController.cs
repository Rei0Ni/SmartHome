using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SmartHome.Application.Interfaces.IPCameras;
using SmartHome.Application.Services;
using SmartHome.Dto.IPCamera;

namespace SmartHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class cameraController : ControllerBase
    {
        private readonly IIPCamerasService _cameraService;
        private readonly HttpClient _client;

        public cameraController(
            IIPCamerasService cameraService,
            IHttpClientFactory clientFactory)
        {
            _cameraService = cameraService;
            _client = clientFactory.CreateClient("IPCameraClient");
        }

        [HttpGet("get/all")]
        [Authorize]
        public async Task<ActionResult<List<IPCameraDto>>> GetAllCameras()
        {
            var cameras = await _cameraService.GetAllCamerasAsync();
            return Ok(cameras);
        }

        [HttpGet("get/{id}")]
        [Authorize]
        public async Task<ActionResult<IPCameraDto>> GetCameraById(Guid id)
        {
            var camera = await _cameraService.GetCameraByIdAsync(id);
            if (camera == null)
            {
                return NotFound();
            }
            return Ok(camera);
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IPCameraDto>> CreateCamera(CreateIPCameraDto camera)
        {
            var createdCamera = await _cameraService.CreateCameraAsync(camera);
            return CreatedAtAction(nameof(GetCameraById), new { id = createdCamera.Id }, createdCamera);
        }

        [HttpPut("update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCamera(UpdateIPCameraDto camera)
        {
            var exists = await _cameraService.CameraExistsAsync(camera.Id);
            if (!exists)
            {
                return NotFound();
            }
            await _cameraService.UpdateCameraAsync(camera);
            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCamera(Guid id)
        {
            var exists = await _cameraService.CameraExistsAsync(id);
            if (!exists)
            {
                return NotFound();
            }
            await _cameraService.DeleteCameraAsync(id);
            return NoContent();
        }

        [HttpGet("stream/{id}")]
        //[Authorize]
        public async Task GetCameraStream(Guid id)
        {
            async Task WriteJsonErrorResponse(int statusCode, string message)
            {
                if (!HttpContext.Response.HasStarted)
                {
                    HttpContext.Response.StatusCode = statusCode;
                    HttpContext.Response.ContentType = "application/json";
                    var errorResponse = new { error =  message };
                    await HttpContext.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
                }
            }

            var camera = await _cameraService.GetCameraConnectionInfo(id);

            if (string.IsNullOrEmpty(camera.StreamUrl))
            {
                await WriteJsonErrorResponse(StatusCodes.Status404NotFound,
                    $"Camera with ID {id} not found or has no valid stream URL.");
                Log.Information("Proxy request failed: Camera {CameraId} not found or URL missing.", id);
                return;
            }

            Log.Information("Attempting to proxy MJPEG stream for camera {CameraId} from URL {StreamUrl}", id, camera.StreamUrl);

            try
            {
                _client.DefaultRequestHeaders.Clear();
                var credentials = $"{camera.Username}:{camera.Password}";
                var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
                _client.DefaultRequestHeaders.Add("Authorization", $"Basic {encodedCredentials}");

                using var cameraResponse = await _client.GetAsync(camera.StreamUrl, HttpCompletionOption.ResponseHeadersRead, HttpContext.RequestAborted);

                if (!cameraResponse.IsSuccessStatusCode)
                {
                    var errorContent = await cameraResponse.Content.ReadAsStringAsync(HttpContext.RequestAborted);
                    await WriteJsonErrorResponse((int)cameraResponse.StatusCode,
                        $"Error fetching stream from camera ID {id}: {cameraResponse.ReasonPhrase} - {errorContent}");
                    Log.Warning("Proxy request failed: Camera {CameraId} returned status {StatusCode} from {MjpegUrl}",
                        id, cameraResponse.StatusCode, camera.StreamUrl);
                    return;
                }

                HttpContext.Response.Headers["Content-Type"] = cameraResponse.Content.Headers.ContentType?.ToString();
                Log.Debug("Proxying stream for camera {CameraId} with Content-Type: {ContentType}",
                    id, HttpContext.Response.Headers["Content-Type"]);

                using var cameraStream = await cameraResponse.Content.ReadAsStreamAsync(HttpContext.RequestAborted);
                await cameraStream.CopyToAsync(HttpContext.Response.Body, HttpContext.RequestAborted);
                Log.Information("Proxy stream completed for camera {CameraId}.", id);
            }
            catch (HttpRequestException ex)
            {
                Log.Error(ex, "Proxy stream HttpRequestException for camera {CameraId} connecting to {MjpegUrl}", id, camera.StreamUrl);
                await WriteJsonErrorResponse(StatusCodes.Status502BadGateway,
                    $"Error connecting to camera stream ID {id}. Ensure camera is online and URL is correct.");
            }
            catch (OperationCanceledException)
            {
                Log.Information("Client disconnected from proxied stream for camera {CameraId}.", id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Proxy stream unexpected error for camera {CameraId}", id);
                await WriteJsonErrorResponse(StatusCodes.Status500InternalServerError,
                    "An internal server error occurred during stream proxying.");
            }
        }
    }
}
