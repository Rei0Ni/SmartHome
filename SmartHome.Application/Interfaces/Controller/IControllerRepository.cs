using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Application.Interfaces.Controller
{
    public interface IControllerRepository
    {
        Task<List<Domain.Entities.Controller>> GetControllers();
        Task<Domain.Entities.Controller> GetController(Guid id);
        Task CreateController(Domain.Entities.Controller createControllerDto);
        Task UpdateController(Domain.Entities.Controller updateControllerDto);
        Task DeleteController(Domain.Entities.Controller deleteController);
    }
}
