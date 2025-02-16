using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Dto.Device;
using SmartHome.Dto.DeviceFunction;
using SmartHome.Dto.DeviceType;

namespace SmartHome.Dto.Dashboard
{
    public class OverviewDeviceDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "Unnamed Device";

        public ICollection<DevicePin> Pins { get; set; } = new List<DevicePin>();
        public Dictionary<string, object> State { get; set; } = new();

        public Guid AreaId { get; set; }
        public DeviceTypeDto DeviceType { get; set; }
        public List<DeviceFunctionDto> DeviceFunctions { get; set; }
    }
}
