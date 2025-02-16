using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Dto;
using SmartHome.Dto.User;

namespace SmartHome.Application.Interfaces.User
{
    public interface IUserService
    {
        Task<ApiResponse<object>> LoginAsync(LoginDto dto);
        Task<UserInfoDto> GetUserProfileAsync(string userId);
    }
}
