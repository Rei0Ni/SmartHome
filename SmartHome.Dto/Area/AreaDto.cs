using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Dto.Controller;

namespace SmartHome.Dto.Area
{
    public class AreaDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ControllerId { get; set; }
        public ICollection<Guid> Devices { get; set; }
    }
}
