using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Dto.User
{
    public class UserWithRolesDto
    {
        public Guid UserId { get; set; } = new Guid();
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? LastLogin { get; set; } = null;
        public string Role { get; set; } = string.Empty; // Holds "Admin", "Normal_User", or "Guest"
    }
}
