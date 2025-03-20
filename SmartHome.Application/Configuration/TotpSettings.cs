using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Application.Configuration
{
    public class TotpSettings
    {
        public string Issuer { get; set; } = "SmartHome System";
        public int CodeLength { get; set; } = 6;
        public int ExpirySeconds { get; set; } = 30;
    }
}
