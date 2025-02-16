using System.Net;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Dto.Area;
using SmartHome.Application.Interfaces.Area;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class areaController : ControllerBase
    {
        public IAreaService _areaService { get; set; }
        public areaController(IAreaService areaService)
        {
            _areaService = areaService;
        }

        // GET: api/<areaController>
        [HttpGet("getall")]
        public async Task<ActionResult<List<AreaDto>>> Get()
        {
            var areas = await _areaService.GetAreas();
            return Ok(areas);
        }

        // GET api/<areaController>/5
        [HttpGet("{Id}/get")]
        public async Task<ActionResult<AreaDto>> Get(string Id)
        {
            var getArea = new GetAreaDto
            {
                Id = new Guid(Id)
            };
            var area = await _areaService.GetArea(getArea);
            if (area == null)
            {
                return NotFound();
            }
            return Ok(area);
        }

        // POST api/<areaController>
        [HttpPost("create")]
        public async Task<ActionResult> Post(CreateAreaDto createAreaDto)
        {
            await _areaService.CreateArea(createAreaDto);
            return CreatedAtAction(nameof(Get), createAreaDto);
        }

        // PUT api/<areaController>/5
        [HttpPut("update")]
        public async Task<ActionResult> Put(UpdateAreaDto updateAreaDto)
        {
            await _areaService.UpdateArea(updateAreaDto);
            return Ok();
        }

        // DELETE api/<areaController>/5
        [HttpDelete("remove")]
        public async Task<ActionResult> Delete(DeleteAreaDto deleteAreaDto)
        {
            await _areaService.DeleteArea(deleteAreaDto);
            return NoContent();
        }
    }
}
