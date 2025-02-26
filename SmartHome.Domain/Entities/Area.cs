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

        // Foreign key to the controller
        public Guid ControllerId { get; set; }
        public ICollection<Guid> Devices { get; set; } = new List<Guid>();
    }
}
