using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Domain.Entities
{
    public class DeviceType
    {
        public Guid Id { get; set; }
        public string Name { get; set; } // e.g., "Lamp", "Temperature Sensor"

        public ICollection<Guid> Functions { get; set; } = new List<Guid>();
        public ICollection<Guid> Devices { get; set; } = new List<Guid>();
    }
}
