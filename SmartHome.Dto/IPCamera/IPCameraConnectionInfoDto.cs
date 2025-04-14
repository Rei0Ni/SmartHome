using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Dto.IPCamera
{
    public class IPCameraConnectionInfoDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string StreamUrl { get; set; }
        public Guid AreaId { get; set; } = Guid.Empty;
    }
}
