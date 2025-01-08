using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Application.DTOs.User;

namespace SmartHome.Application.Interfaces.User
{
    public interface IUserService
    {
        Task<string> LoginAsync(LoginDto dto);
        Task<UserInfoDto> GetUserProfileAsync(string userId);
    }
}
