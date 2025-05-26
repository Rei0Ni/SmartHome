using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Identity.Mongo.Model;

namespace SmartHome.Domain.Entities
{
    public class ApplicationUser : MongoUser<Guid>
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public bool IsEnabled { get; set; } = true;

        public string? TOTPSecret { get; set; }

        public virtual DateTime? LastLogin { get; set; } = null;
        public virtual DateTime? CreationDate { get; set; } = DateTime.Now;
    }
}
