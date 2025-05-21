using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Shared.Interfaces
{
    public interface IHostStatusService
    {
        /// <summary>
        /// Event that is invoked when the host configuration error status changes.
        /// Components can subscribe to this event to trigger UI updates.
        /// </summary>
        event Action? OnHostConfigurationError;
        event Action? OnHostNetworkConnectionError;

        /// <summary>
        /// Gets a value indicating whether there is a current host configuration error.
        /// </summary>
        public static bool HasHostConfigurationError { get; }
        public static bool HasNoNetworkConnectionError { get; }

        /// <summary>
        /// Sets the host configuration error status and invokes the <see cref="OnHostConfigurationError"/> event.
        /// </summary>
        /// <param name="isError">True if there is a host configuration error, false otherwise.</param>
        void SetHostConfigurationError(bool isError);
        void SetNetworkConnectionError(bool isError);
    }
}
