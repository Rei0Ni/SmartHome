using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Dto.Command
{
    public class DeviceCommandDto
    {
        public Guid DeviceId { get; set; }
        public string Function { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }
}
