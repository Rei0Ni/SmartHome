using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Dto.Area;

namespace SmartHome.Application.Interfaces.Area
{
    public interface IAreaRepository
    {
        Task<List<Domain.Entities.Area>> GetAreas();
        Task<Domain.Entities.Area> GetArea(Guid id);
        Task CreateArea(Domain.Entities.Area createAreaDto);
        Task UpdateArea(UpdateAreaDto updateAreaDto);
        Task DeleteArea(DeleteAreaDto deleteArea);
    }
}
