using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Dto.User
{
    public class UserAuthenticationState
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsAuthenticated { get; set; } = false;
        public Dictionary<string, string> Claims { get; set; } = [];
    }
}
