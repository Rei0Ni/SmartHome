using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Enum;

namespace SmartHome.Dto.Log
{
    public class LogDto
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = "No Log Message Here to See."; // e.g., "Device turned on", "Temperature threshold exceeded"
        public string DeviceName { get; set; } = "Unnamed Device"; // e.g., "Living Room Light", "Thermostat"
        public string AreaName { get; set; } = "Unnamed Area"; // e.g., "Living Room", "Kitchen"
        public string UserName { get; set; } = "Unnamed User"; // e.g., "John Doe", "Jane Smith"
        public DateTime Timestamp { get; set; } = DateTime.Now; // Timestamp of the log entry
        public LogLevel Level { get; set; } // e.g., Info, Warning, Error
    }
}
