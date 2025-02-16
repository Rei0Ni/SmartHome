using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Dto.Controller
{
    public class UpdateControllerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string MACAddress { get; set; } // Unique identifier for the ESP32
        public string IPAddress { get; set; } // Last known IP for communication
    }
}
