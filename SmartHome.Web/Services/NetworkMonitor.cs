using SmartHome.Shared.Interfaces;
using System.Diagnostics;

namespace SmartHome.Web.Services
{
    public class NetworkMonitor : INetworkMonitor
    {
        /// <summary>
        /// Gets the current network status, which is always <see cref="NetworkStatus.Internet"/>
        /// for the web application.
        /// </summary>
        public NetworkStatus CurrentStatus => NetworkStatus.Internet;

        /// <summary>
        /// This event is part of the interface but will not actively fire
        /// in this web implementation as the status is static.
        /// </summary>
        public event EventHandler<NetworkStatus> NetworkStatusChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebNetworkMonitor"/> class.
        /// </summary>
        public NetworkMonitor()
        {
            Debug.WriteLine("[WebNetworkMonitor] Initialized. Assuming internet connection always available.");
        }

        /// <summary>
        /// This method does nothing in the web implementation, as the status is static.
        /// </summary>
        public void ForceUpdateStatus()
        {
            Debug.WriteLine("[WebNetworkMonitor] ForceUpdateStatus called (no action taken as status is static).");
            // If you later decide to add a simple JS check for "online/offline"
            // events, you could trigger NetworkStatusChanged here.
        }
    }
}
