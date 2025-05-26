using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Enum;

namespace SmartHome.Dto.DeviceType
{
    public class UpdateDeviceTypeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DeviceTypes Type { get; set; }
        //public ICollection<Guid> Functions { get; set; } = new List<Guid>();
    }
}
