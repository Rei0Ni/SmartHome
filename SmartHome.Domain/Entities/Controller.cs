using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Domain.Entities
{
    public class Controller
    {
        public Guid Id { get; set; }
        public string Name { get; set; } // Human-readable name
        public string MACAddress { get; set; } // Unique identifier for the ESP32
        public string IPAddress { get; set; } // Last known IP for communication
        public bool NeedsReconfiguration { get; set; } // If the Controller Needs to be Reconfigured or Not
        public DateTime LastSeen { get; set; } = DateTime.Now; // Last time the controller was active
        public ICollection<Guid> Areas { get; set; } = new List<Guid>();
    }
}
