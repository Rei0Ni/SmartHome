using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Dto.Command
{
    public class CommandResponseDto
    {
        public string Status { get; set; } // "success" or "error" for the overall command
        public List<DeviceResponseDto> Devices { get; set; } // Array of device responses
    }
}
