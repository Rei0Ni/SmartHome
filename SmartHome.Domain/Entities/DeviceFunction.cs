using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Domain.Entities
{
    public class DeviceFunction
    {
        public Guid Id { get; set; }
        public string Name { get; set; } // e.g., "Turn On", "Set Temperature"

        public ICollection<Guid> DeviceTypes { get; set; } = new List<Guid>();
        public ICollection<Guid> Parameters { get; set; } = new List<Guid>();
    }
}
