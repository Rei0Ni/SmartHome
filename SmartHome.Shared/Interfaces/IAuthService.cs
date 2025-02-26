using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Dto.User;

namespace SmartHome.Shared.Interfaces
{
    public interface IAuthService
    {
        Task<HttpResponseMessage> Login(LoginDto Dto);
        Task<bool> Logout();
        Task<UserInfoDto?> GetCurrentUser();
    }
}
