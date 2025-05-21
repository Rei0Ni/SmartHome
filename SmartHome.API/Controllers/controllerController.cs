using Microsoft.AspNetCore.Mvc;
using SmartHome.Application.Interfaces.Controller;
using SmartHome.Application.Interfaces.ESPConfig;
using SmartHome.Dto.Controller;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class controllerController : ControllerBase
    {
        private readonly IControllerService _controllerService;
        private readonly IESPConfigService _espConfigService;

        public controllerController(IControllerService controllerService, IESPConfigService espConfigService)
        {
            _controllerService = controllerService;
            _espConfigService = espConfigService;
        }

        // GET: api/<controllerController>
        [HttpGet("get/all")]
        public async Task<ActionResult<IEnumerable<ControllerDto>>> Get()
        {
            var controllers = await _controllerService.GetControllers();
            return Ok(controllers);
        }

        // GET api/<controllerController>/5
        [HttpGet("get/{Id}")]
        public async Task<ActionResult<ControllerDto>> Get(string Id)
        {
            var dto = new GetControllerDto
            {
                Id = new Guid(Id)
            };
            var controller = await _controllerService.GetController(dto);
            if (controller == null)
            {
                return NotFound();
            }
            return Ok(controller);
        }

        // POST api/<controllerController>
        [HttpPost("create")]
        public async Task<ActionResult> Post(CreateControllerDto createControllerDto)
        {
            await _controllerService.CreateController(createControllerDto);
            return CreatedAtAction(nameof(Get), createControllerDto);
        }

        // PUT api/<controllerController>/5
        [HttpPut("update")]
        public async Task<ActionResult> Put(UpdateControllerDto updateControllerDto)
        {
            await _controllerService.UpdateController(updateControllerDto);
            return NoContent();
        }

        // DELETE api/<controllerController>/5
        [HttpDelete("delete/{Id}")]
        public async Task<ActionResult> Delete(Guid Id)
        {
            var dto = new DeleteControllerDto { Id = Id };
            await _controllerService.DeleteController(dto);
            return NoContent();
        }

        [HttpPost("/config/update/{Id}")]
        public async Task<ActionResult> UpdateConfig(Guid Id)
        {
            var result = await _espConfigService.UpdateESPCotrollerConfig(Id);
            if (result)
            {
                return Ok(new { Message = "Configuration updated successfully.\nRestarting Controller" });
            }
            else
            {
                return BadRequest(new { Message = "Could not update controller configuration." });
            }
        }
    }
}
