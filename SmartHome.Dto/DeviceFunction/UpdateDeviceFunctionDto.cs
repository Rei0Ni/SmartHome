using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Dto.DeviceFunction
{
    public class UpdateDeviceFunctionDto
    {
        public string Name { get; set; }
        public ICollection<DeviceFunctionParameter> Parameters { get; set; } = new List<DeviceFunctionParameter>();
    }
}
