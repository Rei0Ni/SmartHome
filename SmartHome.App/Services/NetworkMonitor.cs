using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Shared.Interfaces;

namespace SmartHome.App.Services
{
    public class NetworkMonitor : INetworkMonitor, IDisposable
    {
        private readonly IConnectivity _connectivity;
        private NetworkStatus _currentStatus;

        /// <summary>
        /// Event that fires when the network status changes.
        /// </summary>
        public event EventHandler<NetworkStatus> NetworkStatusChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="MauiNetworkMonitor"/> class.
        /// </summary>
        /// <param name="connectivity">The MAUI Connectivity service.</param>
        public NetworkMonitor(IConnectivity connectivity)
        {
            _connectivity = connectivity ?? throw new ArgumentNullException(nameof(connectivity));
            // Subscribe to MAUI's connectivity changed event
            _connectivity.ConnectivityChanged += OnMauiConnectivityChanged;
            // Perform an initial status update
            UpdateStatusFromMaui();
        }

        /// <summary>
        /// Gets the current network status.
        /// </summary>
        public NetworkStatus CurrentStatus
        {
            get => _currentStatus;
            private set
            {
                // Only update and raise event if the status has actually changed
                if (_currentStatus != value)
                {
                    _currentStatus = value;
                    NetworkStatusChanged?.Invoke(this, _currentStatus);
                    Console.WriteLine($"[MauiNetworkMonitor] NetworkStatus changed: {_currentStatus}");
                }
            }
        }

        /// <summary>
        /// Forces an update of the network status by re-evaluating the MAUI connectivity.
        /// </summary>
        public void ForceUpdateStatus()
        {
            UpdateStatusFromMaui();
        }

        /// <summary>
        /// Updates the internal network status based on the MAUI Connectivity API.
        /// </summary>
        private void UpdateStatusFromMaui()
        {
            var mauiAccess = _connectivity.NetworkAccess;

            // Map MAUI's NetworkAccess enum to the shared NetworkStatus enum
            CurrentStatus = mauiAccess switch
            {
                NetworkAccess.Internet => NetworkStatus.Internet,
                NetworkAccess.ConstrainedInternet => NetworkStatus.LimitedInternet,
                NetworkAccess.Local => NetworkStatus.LocalOnly,
                NetworkAccess.None => NetworkStatus.NoConnection,
                _ => NetworkStatus.Unknown // Fallback for unknown states
            };
        }

        /// <summary>
        /// Event handler for MAUI's ConnectivityChanged event.
        /// </summary>
        private void OnMauiConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            UpdateStatusFromMaui(); // Re-evaluate and update status
        }

        /// <summary>
        /// Disposes of resources, specifically unsubscribing from the ConnectivityChanged event.
        /// </summary>
        public void Dispose()
        {
            // Unsubscribe to prevent memory leaks
            _connectivity.ConnectivityChanged -= OnMauiConnectivityChanged;
            GC.SuppressFinalize(this); // Suppress finalization as we've cleaned up managed resources
        }
    }
}
