using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Shared.Interfaces
{
    /// <summary>
    /// Defines the possible network connectivity states.
    /// </summary>
    public enum NetworkStatus
    {
        Unknown,
        NoConnection,
        LocalOnly,
        LimitedInternet,
        Internet
    }

    /// <summary>
    /// Interface for monitoring network connectivity.
    /// This interface is implemented differently for MAUI and Web platforms.
    /// </summary>
    public interface INetworkMonitor
    {
        /// <summary>
        /// Gets the current network status.
        /// </summary>
        NetworkStatus CurrentStatus { get; }

        /// <summary>
        /// Event that fires when the network status changes.
        /// </summary>
        event EventHandler<NetworkStatus> NetworkStatusChanged;

        /// <summary>
        /// Forces an update of the network status.
        /// Implementations should re-evaluate the current network state.
        /// </summary>
        void ForceUpdateStatus();
    }
}
