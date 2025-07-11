﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Dto.Device;
using SmartHome.Dto.IPCamera;

namespace SmartHome.Application.Interfaces.Device
{
    public interface IDeviceDataService
    {
        Task<List<DeviceDto>> GetDevicesForAreas(List<Guid> areaIds);
        Task<List<IPCameraDto>> GetIPCamerasForAreas(List<Guid> areaIds);
    }
}
