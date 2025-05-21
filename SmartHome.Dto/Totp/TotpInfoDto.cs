using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Dto.Totp
{
    public class TotpInfoDto
    {
        public Guid UserId { get; set; }
        public string SecretKey { get; set; } = string.Empty;
        public string TotpQRUri { get; set; } = string.Empty;
    }
}
