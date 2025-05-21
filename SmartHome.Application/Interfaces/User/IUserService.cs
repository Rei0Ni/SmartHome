using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Dto;
using SmartHome.Dto.Totp;
using SmartHome.Dto.User;
using SmartHome.Enum;

namespace SmartHome.Application.Interfaces.User
{
    public interface IUserService
    {
        Task<ApiResponse<object>> LoginAsync(LoginDto dto);
        Task<UserAuthenticationState> GetUserAuthenticationStateAsync(string userId);
        Task<ApiResponse<object>> GetAllUsersAsync(Guid currentUserId);
        Task<ApiResponse<object>> GetUserData(Guid userId);
        Task<ApiResponse<object>> CreateAdminUserAsync(RegisterAdminUserDto dto);
        Task<ApiResponse<object>> CreateNormalUserAsync(RegisterUserDto dto);
        //Task<ApiResponse<object>> CreateGuestUserAsync(RegisterUserDto dto);
        Task<ApiResponse<object>> CreateUserAsync(RegisterUserDto dto, Role role);
        Task<ApiResponse<TotpInfoDto>> GetUserTotpDetailsAsync(string userId);
        Task<ApiResponse<object>> UpdateAdminUserProfileAsync(UpdateAdminUserProfileDto dto);
        Task<ApiResponse<object>> UpdateUserProfileAsync(UpdateUserProfileDto dto);
        Task<ApiResponse<object>> UpdateUserPasswordAsync(UpdatePasswordDto dto);
        Task<FileStreamResult> GetProfilePictureAsync(string userId);
        Task<ApiResponse<object>> UpdateProfilePictureAsync(UpdateProfilePictureDto dto, string userId);
        Task<ApiResponse<object>> DeleteUserAsync(Guid userId);
    }
}
