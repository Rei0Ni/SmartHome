using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Dto.DeviceFunction;
using SmartHome.Dto.DeviceType;

namespace SmartHome.Dto.Device
{
    public class DeviceDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "Unnamed Device";
        public string Description { get; set; } = "No description available";
        public string Brand { get; set; } = "Unknown";
        public string Model { get; set; } = "Generic";
        public string Manufacturer { get; set; } = "Unknown";
        public string SerialNumber { get; set; } = "Unknown";

        public ICollection<DevicePin> Pins { get; set; } = new List<DevicePin>();
        public Dictionary<string, object> State { get; set; } = new();

        public Guid DeviceTypeId { get; set; }
        public DeviceTypeDto DeviceType { get; set; }
        public List<DeviceFunctionDto> DeviceFunctions { get; set; }

        public DateTime LastUpdated { get; set; }
        public Guid AreaId { get; set; }
    }
}
