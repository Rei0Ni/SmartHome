using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Dto.Device;

namespace SmartHome.Dto.Dashboard
{
    public class OverviewAreaDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ControllerId { get; set; }
        public List<OverviewDeviceDto> AreaDevices { get; set; } = new List<OverviewDeviceDto>();
        public List<OverviewCameraDto> AreaCameras { get; set; } = new List<OverviewCameraDto>();
    }
}
