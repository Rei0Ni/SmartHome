using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Domain.Entities
{
    public class UserAreas
    {
        public Guid UserId { get; set; }
        public List<Guid> AllowedAreaIds { get; set; } = new List<Guid>();
    }
}
