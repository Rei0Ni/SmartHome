using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Dto.Area
{
    public class CreateAreaDto
    {
        public Guid ControllerId { get; set; }
        public string Name { get; set; }
    }
}
