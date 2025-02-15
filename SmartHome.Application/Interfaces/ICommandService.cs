using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Dto.Command;

namespace SmartHome.Application.Interfaces
{
    public interface ICommandService
    {
        Task<CommandResponseDto> SendCommandAsync(string controllerIp, CommandRequestDto commandRequest);
    }
}
