using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace SmartHome.Shared.Interfaces
{
    public interface IHubService : IAsyncDisposable
    {
        Task StartAsync(string endpointPath);
        Task StopAsync();
        bool IsConnected { get; }
        HubConnectionState State { get; }

        void On<T>(string methodName, Action<T> handler);
        void On<T1, T2>(string methodName, Action<T1, T2> handler);
        void On<T1, T2, T3>(string methodName, Action<T1, T2, T3> handler);
        void On<T1, T2, T3, T4>(string methodName, Action<T1, T2, T3, T4> handler);
        // Add more overloads as needed for different numbers of parameters

        Task InvokeAsync(string methodName, params object[] args);
    }
}
