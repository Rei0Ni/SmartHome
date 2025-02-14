using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Dto.Device
{
    public class CreateDeviceDto
    {
        public Guid AreaId { get; set; }
        public Guid DeviceTypeId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; } = "No description available";
        public string? Brand { get; set; } = "Unknown";
        public string? Model { get; set; } = "Generic";
        public string? Manufacturer { get; set; } = "Unknown";
        public string? SerialNumber { get; set; } = "Unknown";
        public ICollection<DevicePin> Pins { get; set; } = new List<DevicePin>();
    }
}
