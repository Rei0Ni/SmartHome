using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Application.Interfaces.Device
{
    public interface IDeviceRepository
    {
        Task<List<Domain.Entities.Device>> GetDevices();
        Task<Domain.Entities.Device> GetDevice(Guid id);
        Task CreateDevice(Domain.Entities.Device createDeviceDto);
        Task UpdateDevice(Domain.Entities.Device updateDeviceDto);
        Task DeleteDevice(Domain.Entities.Device deleteDevice);
    }
}
