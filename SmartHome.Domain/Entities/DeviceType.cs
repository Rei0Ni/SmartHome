using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Enum;

namespace SmartHome.Domain.Entities
{
    public class DeviceType
    {
        public Guid Id { get; set; }
        public string Name { get; set; } // e.g., "Lamp", "Temperature Sensor"
        public DeviceTypes Type { get; set; }

        public ICollection<Guid> Devices { get; set; } = new List<Guid>();
    }
}
