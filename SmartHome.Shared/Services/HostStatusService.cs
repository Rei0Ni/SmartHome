using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SmartHome.Shared.Interfaces;

namespace SmartHome.Shared.Services
{
    public class HostStatusService : IHostStatusService // Implement the new interface
    {
        
        public event Action? OnHostConfigurationError;
        public event Action? OnHostNetworkConnectionError;

        public INetworkMonitor _networkMonitor;

        public HostStatusService(INetworkMonitor networkMonitor)
        {
            _networkMonitor = networkMonitor;
        }

        public static bool HasHostConfigurationError { get; private set; }
        public static bool HasNoNetworkConnectionError { get; private set; }

        public void SetHostConfigurationError(bool isError)
        {
            // Only update and invoke if the status has actually changed to avoid unnecessary re-renders.
            if (HasHostConfigurationError != isError)
            {
                HasHostConfigurationError = isError;
                // Invoke the event to notify all subscribed components.
                OnHostConfigurationError?.Invoke();
            }
        }

        public void SetNetworkConnectionError(bool isError)
        {
            // Only update and invoke if the status has actually changed to avoid unnecessary re-renders.
            if (HasNoNetworkConnectionError != isError)
            {
                HasNoNetworkConnectionError = isError;
                // Invoke the event to notify all subscribed components.
                OnHostNetworkConnectionError?.Invoke();
            }
        }
    }
}
