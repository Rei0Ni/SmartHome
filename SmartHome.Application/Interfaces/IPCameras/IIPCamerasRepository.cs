using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Domain.Entities;

namespace SmartHome.Application.Interfaces.IPCameras
{
    public interface IIPCamerasRepository
    {
        Task<List<IPCamera>> GetAllCamerasAsync();
        Task<List<IPCamera>> GetCamerasForAreas(List<Guid> areaIds);
        Task<IPCamera> GetCameraByIdAsync(Guid id);
        Task<string> GetCameraStreamUrl(Guid id);
        Task<IPCamera> CreateCameraAsync(IPCamera camera);
        Task<IPCamera> UpdateCameraAsync(IPCamera camera);
        Task<bool> DeleteCameraAsync(Guid id);
        Task<bool> CameraExistsAsync(Guid id);
    }
}
