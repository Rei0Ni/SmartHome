using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Domain.Entities
{
    public class Device
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Guid DeviceTypeId { get; set; } // Foreign key
        public DeviceType DeviceType { get; set; }

        public string Model { get; set; }
        public DateTime LastUpdated { get; set; }

        public Guid AreaId { get; set; }
    }
}
