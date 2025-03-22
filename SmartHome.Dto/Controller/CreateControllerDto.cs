using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Dto.Controller
{
    public class CreateControllerDto
    {
        [Required(ErrorMessage = "Name is Required")]
        [MaxLength(50, ErrorMessage = "Name must not exceed 50 characters.")]
        public string Name { get; set; } // Human-readable name
        [Required(ErrorMessage = "MAC Address is Required")]
        [RegularExpression(@"^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$", ErrorMessage = "Invalid MAC Address")]
        public string MACAddress { get; set; } // Unique identifier for the ESP32
        [Required(ErrorMessage = "IP Address is Required")]
        [RegularExpression(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b", ErrorMessage = "Invalid IP Address")]
        public string IPAddress { get; set; } // Last known IP for communication
    }
}
