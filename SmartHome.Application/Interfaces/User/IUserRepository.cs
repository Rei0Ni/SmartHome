using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Domain.Entities;

namespace SmartHome.Application.Interfaces.User
{
    public interface IUserRepository
    {
        Task<bool> UpdateLastLoginValue(ApplicationUser user);
    }
}
