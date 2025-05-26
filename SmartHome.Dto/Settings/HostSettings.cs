using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Dto.Settings
{
    public class HostSettings
    {
        [Required(ErrorMessage = "Primary Host is required")]
        [Url(ErrorMessage = "Primary Host must be a valid URL.")]
        [StringLength(255, ErrorMessage = "Primary Host must not exceed 255 charachters.")]
        public string PrimaryHost { get; set; } = "";
        [Required(ErrorMessage = "Secondary Host is required")]
        [Url(ErrorMessage = "Secondary Host must be a valid URL.")]
        [StringLength(255, ErrorMessage = "Secondary Host must not exceed 255 charachters.")]
        public string SecondaryHost { get; set; } = "";
    }
}
