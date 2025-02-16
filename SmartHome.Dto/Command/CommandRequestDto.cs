using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Dto.Command
{
    public class CommandRequestDto
    {
        public Guid AreaId { get; set; }
        public Guid ControllerId { get; set; }
        public List<DeviceCommandDto> Devices { get; set; }
    }
}
