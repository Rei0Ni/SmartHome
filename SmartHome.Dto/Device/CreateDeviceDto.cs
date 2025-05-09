using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Dto.Device
{
    public class CreateDeviceDto
    {
        public Guid AreaId { get; set; }
        public Guid DeviceTypeId { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        [MaxLength(50, ErrorMessage = "Name must not exceed 50 characters.")]
        public string Name { get; set; }
        [MaxLength(200, ErrorMessage = "Description must not exceed 200 characters.")]
        public string? Description { get; set; } = "No description available";
        [MaxLength(50, ErrorMessage = "Brand must not exceed 50 characters.")]
        public string? Brand { get; set; } = "Unknown";
        [MaxLength(50, ErrorMessage = "Model must not exceed 50 characters.")]
        public string? Model { get; set; } = "Generic";
        [MaxLength(50, ErrorMessage = "Manufacturer must not exceed 50 characters.")]
        public string? Manufacturer { get; set; } = "Unknown";
        [MaxLength(50, ErrorMessage = "Serial Number must not exceed 50 characters.")]
        public string? SerialNumber { get; set; } = "Unknown";
        [Required(ErrorMessage = "Pins are Required")]
        public List<DevicePin> Pins { get; set; } = new List<DevicePin>();
    }
}
