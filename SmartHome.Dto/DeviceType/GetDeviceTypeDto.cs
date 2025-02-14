using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Dto.DeviceType
{
    public class GetDeviceTypeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<Guid> Functions { get; set; } = new List<Guid>();
    }
}
