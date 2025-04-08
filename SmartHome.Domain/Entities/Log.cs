using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Enum;

namespace SmartHome.Domain.Entities
{
    public class Log
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = "No Log Message Here to See."; // e.g., "Device turned on", "Temperature threshold exceeded"
        public string DeviceName { get; set; } = "Unnamed Device"; // e.g., "Living Room Light", "Thermostat"
        public string AreaName { get; set; } = "Unnamed Area"; // e.g., "Living Room", "Kitchen"
        public DateTime Timestamp { get; set; } = DateTime.Now; // Timestamp of the log entry
        public LogLevel Level { get; set; } // e.g., Info, Warning, Error
        public Guid? UserId { get; set; } // ID of the user associated with the log entry
        public string? UserName { get; set; } = "Unnamed User"; // e.g., "John Doe"
        public Guid? AreaId { get; set; } // ID of the Area of the Device associated with the log entry
        public Guid? DeviceId { get; set; } // ID of the device associated with the log entry
    }
}
