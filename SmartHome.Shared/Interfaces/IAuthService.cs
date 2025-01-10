using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Shared.Models.Auth;

namespace SmartHome.Shared.Interfaces
{
    internal interface IAuthService
    {
        Task<HttpResponseMessage> Login(LoginDto Dto);
        Task<bool> Logout();
        Task<UserInfoDto?> GetCurrentUser();
    }
}
