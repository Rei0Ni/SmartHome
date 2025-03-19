using Microsoft.AspNetCore.Mvc;
using SmartHome.Application.Interfaces.DeviceFunction;
using SmartHome.Dto.DeviceFunction;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class devicefunctionController : ControllerBase
    {
        private readonly IDeviceFunctionService _deviceFunctionService;

        public devicefunctionController(IDeviceFunctionService deviceFunctionService)
        {
            _deviceFunctionService = deviceFunctionService;
        }

        // GET: api/<devicefunctionsController>
        [HttpGet("get/all")]
        public async Task<ActionResult<IEnumerable<DeviceFunctionDto>>> Get()
        {
            var deviceFunctions = await _deviceFunctionService.GetDeviceFunctions();
            return Ok(deviceFunctions);
        }

        // GET api/<devicefunctionsController>/5
        [HttpGet("get/{id}")]
        public async Task<ActionResult<DeviceFunctionDto>> Get(Guid id)
        {
            var deviceFunction = await _deviceFunctionService.GetDeviceFunction(id);
            if (deviceFunction == null)
            {
                return NotFound();
            }
            return Ok(deviceFunction);
        }

        // POST api/<devicefunctionsController>
        [HttpPost("create")]
        public async Task<ActionResult> Post(CreateDeviceFunctionDto createDeviceFunctionDto)
        {
            await _deviceFunctionService.CreateDeviceFunction(createDeviceFunctionDto);
            return CreatedAtAction(nameof(Get), createDeviceFunctionDto);
        }

        // PUT api/<devicefunctionsController>/5
        [HttpPut("{id}/update")]
        public async Task<ActionResult> Put(Guid id, UpdateDeviceFunctionDto updateDeviceFunctionDto)
        {
            var deviceFunction = await _deviceFunctionService.GetDeviceFunction(id);
            if (deviceFunction == null)
            {
                return NotFound();
            }
            await _deviceFunctionService.UpdateDeviceFunction(updateDeviceFunctionDto);
            return NoContent();
        }

        // DELETE api/<devicefunctionsController>/5
        [HttpDelete("{id}/delete")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var deviceFunction = await _deviceFunctionService.GetDeviceFunction(id);
            if (deviceFunction == null)
            {
                return NotFound();
            }
            await _deviceFunctionService.DeleteDeviceFunction(new DeleteDeviceFunctionDto { Id = id });
            return NoContent();
        }
    }
}
