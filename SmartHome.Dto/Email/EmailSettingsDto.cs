using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Dto.Email
{
    public class EmailSettingsDto
    {
        public Guid Id { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public bool UseSsl { get; set; }

    }
}
