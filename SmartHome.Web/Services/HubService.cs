using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using SmartHome.Shared.Interfaces;

namespace SmartHome.Web.Services
{
    public class HubService : IHubService
    {
        private HubConnection? _hubConnection;
        private readonly NavigationManager _navigationManager;
        private readonly IJSRuntime _jsRuntime;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly IConfiguration _configuration;
        private readonly ILogger<HubService> _logger;
        private readonly IJwtStorageService _jwtStorageService; // Inject JWT storage service

        public HubService(NavigationManager navigationManager, IJSRuntime jsRuntime, AuthenticationStateProvider authenticationStateProvider, IConfiguration configuration, ILogger<HubService> logger, IJwtStorageService jwtStorageService)
        {
            _navigationManager = navigationManager;
            _jsRuntime = jsRuntime;
            _authenticationStateProvider = authenticationStateProvider;
            _configuration = configuration;
            _logger = logger;
            _jwtStorageService = jwtStorageService;
        }

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;
        public HubConnectionState State => _hubConnection?.State ?? HubConnectionState.Disconnected;

        public async Task StartAsync(string endpointPath = "/wss/overview") // Added optional endpointPath parameter
        {
            if (_hubConnection == null)
            {
                var hubUrl = $"https://{_configuration["MainHost"]}:62061" + endpointPath;

                string? accessToken = await _jwtStorageService.GetTokenAsync(); // Get token using the injected service

                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(hubUrl, options =>
                    {
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            options.AccessTokenProvider = () => Task.FromResult(accessToken);
                        }
                    })
                    .WithAutomaticReconnect()
                    .Build();

                _hubConnection.Reconnecting += error =>
                {
                    _logger.LogWarning($"Reconnecting to hub: {error?.Message}");
                    return Task.CompletedTask;
                };

                _hubConnection.Reconnected += connectionId =>
                {
                    _logger.LogInformation($"Successfully reconnected to hub with connection ID: {connectionId}");
                    return Task.CompletedTask;
                };

                _hubConnection.Closed += error =>
                {
                    _logger.LogError($"Hub connection closed: {error?.Message}");
                    return Task.CompletedTask;
                };
            }

            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                try
                {
                    await _hubConnection.StartAsync();
                    _logger.LogInformation($"Hub connection started at {endpointPath}.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error starting hub connection at {endpointPath}.");
                }
            }
        }

        public async Task StopAsync()
        {
            if (_hubConnection != null && _hubConnection.State == HubConnectionState.Connected)
            {
                try
                {
                    await _hubConnection.StopAsync();
                    _logger.LogInformation("Hub connection stopped.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error stopping hub connection.");
                }
            }
        }

        public void On<T>(string methodName, Action<T> handler)
        {
            EnsureConnectionStarted();
            _hubConnection!.On(methodName, handler);
        }

        public void On<T1, T2>(string methodName, Action<T1, T2> handler)
        {
            EnsureConnectionStarted();
            _hubConnection!.On(methodName, handler);
        }

        public void On<T1, T2, T3>(string methodName, Action<T1, T2, T3> handler)
        {
            EnsureConnectionStarted();
            _hubConnection!.On(methodName, handler);
        }

        public void On<T1, T2, T3, T4>(string methodName, Action<T1, T2, T3, T4> handler)
        {
            EnsureConnectionStarted();
            _hubConnection!.On(methodName, handler);
        }

        private void EnsureConnectionStarted()
        {
            if (_hubConnection == null || _hubConnection.State != HubConnectionState.Connected)
            {
                throw new InvalidOperationException("Hub connection is not started. Call StartAsync first.");
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.DisposeAsync();
            }
        }

        public async Task InvokeAsync(string methodName, params object[] args)
        {
            EnsureConnectionStarted();
            await _hubConnection!.InvokeAsync(methodName, args);
        }
    }
}
