using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Dto.Controller;

namespace SmartHome.Application.Interfaces.Controller
{
    public interface IControllerService
    {
        Task<List<ControllerDto>> GetControllers();
        Task<ControllerDto> GetController(GetControllerDto getController);
        Task CreateController(CreateControllerDto createControllerDto);
        Task UpdateController(UpdateControllerDto updateControllerDto);
        Task DeleteController(DeleteControllerDto deleteController);
    }
}
