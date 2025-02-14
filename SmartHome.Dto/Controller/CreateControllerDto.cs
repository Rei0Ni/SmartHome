using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Dto.Controller
{
    public class CreateControllerDto
    {
        public string Name { get; set; } // Human-readable name
        public string MACAddress { get; set; } // Unique identifier for the ESP32
        public string IPAddress { get; set; } // Last known IP for communication
    }
}
