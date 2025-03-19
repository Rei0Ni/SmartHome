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
        Task<List<Domain.Entities.Area>> GetAllAreas();
        Task<Domain.Entities.Area> GetArea(Guid id);
        Task CreateArea(Domain.Entities.Area createAreaDto);
        Task UpdateArea(Domain.Entities.Area updateAreaDto);
        Task DeleteArea(Domain.Entities.Area deleteArea);
    }
}
