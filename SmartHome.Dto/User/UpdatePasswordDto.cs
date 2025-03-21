using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Dto.Annotations;

namespace SmartHome.Dto.User
{
    public class UpdatePasswordDto
    {
        [Required(ErrorMessage = "⚠️ Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "New Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Retype Password is required")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string RetypePassword { get; set; }

        [Required(ErrorMessage = "TOTP is required.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "TOTP must be a 6-digit number.")]
        public string Totp { get; set; }
    }
}
