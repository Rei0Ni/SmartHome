using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Domain.Entities
{
    public class IPCamera
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "Unnamed Camera";
        public string IPAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string StreamUrl { get; set; }
        public DateTime LastAccessed { get; set; }

        public Guid AreaId { get; set; }
    }
}
