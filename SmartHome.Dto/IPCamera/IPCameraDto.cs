using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Dto.IPCamera
{
    public class IPCameraDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "Unnamed Camera";
        //public string StreamUrl { get; set; } = "http//default-stream-url";
        public DateTime LastAccessed { get; set; }
        public Guid AreaId { get; set; }
    }
}
