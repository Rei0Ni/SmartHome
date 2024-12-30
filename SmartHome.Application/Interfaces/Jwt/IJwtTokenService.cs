using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Application.Interfaces.Jwt
{
    public interface IJwtTokenService
    {
        string GenerateJwtToken(string userId, string username, string email, IList<string> roles);
        void ValidateJwtToken(string token);
    }
}
