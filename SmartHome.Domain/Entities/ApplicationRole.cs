using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Identity.Mongo.Model;

namespace SmartHome.Domain.Entities
{
    public class ApplicationRole() : MongoRole<Guid>
    {
    }
}
