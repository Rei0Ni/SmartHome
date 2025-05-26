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
        public string Name { get; set; } = "Unnamed Device";
        public string Description { get; set; } = "No description available";
        public string Brand { get; set; } = "Unknown";
        public string Model { get; set; } = "Generic";
        public string Manufacturer { get; set; } = "Unknown";
        public string SerialNumber { get; set; } = "Unknown";

        public int Pin { get; set; }
        public Dictionary<string, object> State { get; set; } = new();
        public Guid DeviceTypeId { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public Guid AreaId { get; set; }
    }
}
