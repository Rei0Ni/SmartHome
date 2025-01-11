using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Shared.Models.Auth
{
    public class LoginErrorResponse
    {
        public string Field { get; set; }
        public string Error { get; set; }
    }
}
