using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Dto.Area;

namespace SmartHome.Application.Interfaces.Area
{
    public interface IAreaService
    {
        Task<List<AreaDto>> GetAreas();
        Task<AreaDto> GetArea(GetAreaDto getArea);
        Task CreateArea(CreateAreaDto createAreaDto);
        Task UpdateArea(UpdateAreaDto updateAreaDto);
        Task DeleteArea(DeleteAreaDto deleteArea);
    }
}
