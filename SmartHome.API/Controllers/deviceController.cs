using Microsoft.AspNetCore.Mvc;
using SmartHome.Application.Interfaces.Device;
using SmartHome.Dto.Device;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class deviceController : ControllerBase
    {
        private readonly IDeviceService _deviceService;

        public deviceController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        // GET: api/device
        [HttpGet("get/all")]
        public async Task<ActionResult<IEnumerable<DeviceDto>>> Get()
        {
            var devices = await _deviceService.GetDevices();
            return Ok(devices);
        }

        // GET api/device/{id}
        [HttpGet("get/{id}")]
        public async Task<ActionResult<DeviceDto>> Get(Guid id)
        {
            var device = await _deviceService.GetDevice(id);
            if (device == null)
            {
                return NotFound();
            }
            return Ok(device);
        }

        // POST api/device
        [HttpPost("create")]
        public async Task<ActionResult> Post([FromBody] CreateDeviceDto createDeviceDto)
        {
            await _deviceService.CreateDevice(createDeviceDto);
            return CreatedAtAction(nameof(Get), createDeviceDto);
        }

        // PUT api/device/{id}
        [HttpPut("{id}/update")]
        public async Task<ActionResult> Put(Guid id, [FromBody] UpdateDeviceDto updateDeviceDto)
        {
            if (id != updateDeviceDto.Id)
            {
                return BadRequest();
            }

            await _deviceService.UpdateDevice(updateDeviceDto);
            return NoContent();
        }

        // DELETE api/device/{id}
        [HttpDelete("{id}/delete")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var deleteDeviceDto = new DeleteDeviceDto { Id = id };
            await _deviceService.DeleteDevice(deleteDeviceDto);
            return NoContent();
        }
    }
}
