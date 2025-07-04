﻿using System.Net;
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
        [HttpGet("get/all")]
        public async Task<ActionResult<List<AreaDto>>> Get()
        {
            var areas = await _areaService.GetAllAreas();
            return Ok(areas);
        }

        // GET api/<areaController>/5
        [HttpGet("get/{Id}")]
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
            return NoContent();
        }

        // DELETE api/<areaController>/5
        [HttpDelete("delete/{Id}")]
        public async Task<ActionResult> Delete(Guid Id)
        {
            var dto = new DeleteAreaDto(){ Id = Id };
            await _areaService.DeleteArea(dto);
            return NoContent();
        }
    }
}
