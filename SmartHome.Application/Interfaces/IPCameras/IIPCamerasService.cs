using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Dto.IPCamera;

namespace SmartHome.Application.Interfaces.IPCameras
{
    public interface IIPCamerasService
    {
        Task<List<IPCameraDto>> GetAllCamerasAsync();
        Task<IPCameraDto> GetCameraByIdAsync(Guid id);
        Task<string> GetCameraStreamUrl(Guid id);
        Task<IPCameraConnectionInfoDto> GetCameraConnectionInfo(Guid id);
        Task<IPCameraDto> CreateCameraAsync(CreateIPCameraDto camera);
        Task<IPCameraDto> UpdateCameraAsync(UpdateIPCameraDto camera);
        Task<bool> DeleteCameraAsync(Guid id);
        Task<bool> CameraExistsAsync(Guid id);
    }
}
