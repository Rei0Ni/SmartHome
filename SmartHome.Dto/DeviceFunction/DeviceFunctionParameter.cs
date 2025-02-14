using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Enum;

namespace SmartHome.Dto.DeviceFunction
{
    public class DeviceFunctionParameter
    {
        public string Name { get; set; } // e.g., "Speed Level", "Temperature"
        public ParameterType Type { get; set; } // Enum for data type (Integer, Boolean, etc.)
    }
}
