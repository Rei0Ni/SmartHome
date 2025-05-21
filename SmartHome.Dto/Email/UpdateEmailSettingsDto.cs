using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Dto.Email
{
    public class UpdateEmailSettingsDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "SMTP Server must not exceed 255 characters.")]
        public string SmtpServer { get; set; }

        [Range(1, 65535, ErrorMessage = "Port must be between 1 and 65535.")]
        public int Port { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string SenderEmail { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Sender Name must not exceed 100 characters.")]
        public string SenderName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Username must not exceed 100 characters.")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Password must not exceed 100 characters.")]
        public string Password { get; set; }
        [Required]
        public bool UseSsl { get; set; }

    }
}
