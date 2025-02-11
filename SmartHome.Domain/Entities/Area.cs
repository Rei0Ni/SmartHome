using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Domain.Entities
{
    public class Area
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "Unnamed Area";

        //public virtual ICollection<Device> Devices { get; set; }
    }
}
