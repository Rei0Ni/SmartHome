using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Dto.ESPConfig;

namespace SmartHome.Application.Interfaces.ESPConfig
{
    public interface IESPConfigService
    {
        Task<bool> UpdateESPCotrollerConfig(Guid controllerId);
    }
}
