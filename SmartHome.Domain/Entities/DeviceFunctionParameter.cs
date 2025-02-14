using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Enum;

namespace SmartHome.Domain.Entities
{
    public class DeviceFunctionParameter
    {
        public Guid Id { get; set; }
        public string Name { get; set; } // e.g., "Speed Level", "Temperature"
        public ParameterType Type { get; set; } // Enum for data type (Integer, Boolean, etc.)

        public Guid DeviceFunctionId { get; set; }
    }
}
