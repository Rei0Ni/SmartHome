using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Dto.User
{
    public class UserDto
    {
        public Guid Id { get; set; } = new Guid();
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<Guid> AllowedAreas { get; set; } = new List<Guid>();
        //public string? ProfilePictureUrl { get; set; }
    }
}
