using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Application.Interfaces.Hubs
{
    public interface IHubState
    {
        void AddConnectedUser(string userId);
        void RemoveConnectedUser(string userId);
        List<string> GetConnectedUsers();
    }
}
