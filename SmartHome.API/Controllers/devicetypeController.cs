using Microsoft.AspNetCore.Mvc;
using SmartHome.Application.Interfaces.DeviceType;
using SmartHome.Dto.DeviceType;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class devicetypeController : ControllerBase
    {
        public IDeviceTypeService _deviceTypeService { get; set; }
        public devicetypeController(IDeviceTypeService deviceTypeService)
        {
            _deviceTypeService = deviceTypeService;
        }

        // GET: api/<devicetypesController>
        [HttpGet("get/all")]
        public async Task<ActionResult<IEnumerable<GetDeviceTypeDto>>> Get()
        {
            var deviceTypes = await _deviceTypeService.GetDeviceTypes();
            return Ok(deviceTypes);
        }

        // GET api/<devicetypesController>/5
        [HttpGet("get/{id}")]
        public async Task<ActionResult<GetDeviceTypeDto>> Get(Guid id)
        {
            var deviceType = await _deviceTypeService.GetDeviceType(id);
            if (deviceType == null)
            {
                return NotFound();
            }
            return Ok(deviceType);
        }

        // POST api/<devicetypesController>
        [HttpPost("create")]
        public async Task<ActionResult> Post(CreateDeviceTypeDto createDeviceTypeDto)
        {
            await _deviceTypeService.CreateDeviceType(createDeviceTypeDto);
            return CreatedAtAction(nameof(Get), new { id = createDeviceTypeDto.Name }, createDeviceTypeDto);
        }

        // PUT api/<devicetypesController>/5
        [HttpPut("update")]
        public async Task<ActionResult> Put(UpdateDeviceTypeDto dto)
        {
            var deviceType = _deviceTypeService.GetDeviceType(dto.Id);
            if (deviceType == null)
            {
                return BadRequest();
            }

            await _deviceTypeService.UpdateDeviceType(dto);
            return NoContent();
        }

        // DELETE api/<devicetypesController>/5
        [HttpDelete("delete/{Id}")]
        public async Task<ActionResult> Delete(Guid Id)
        {
            var deleteDeviceTypeDto = new DeleteDeviceTypeDto { Id = Id };
            await _deviceTypeService.DeleteDeviceType(deleteDeviceTypeDto);
            return NoContent();
        }
    }
}
