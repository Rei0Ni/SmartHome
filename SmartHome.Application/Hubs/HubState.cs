using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Application.Interfaces.Hubs;

namespace SmartHome.Application.Hubs
{
    public class HubState : IHubState
    {
        // Use a ConcurrentDictionary for thread safety.
        private static readonly ConcurrentDictionary<string, bool> ConnectedUserIds = new ConcurrentDictionary<string, bool>();

        public void AddConnectedUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
            }
            ConnectedUserIds.TryAdd(userId, true);
        }

        public List<string> GetConnectedUsers()
        {
            return ConnectedUserIds.Keys.ToList();
        }

        public void RemoveConnectedUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
            }
            ConnectedUserIds.TryRemove(userId, out _);
        }
    }
}