using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using SmartHome.Shared.Interfaces;

namespace SmartHome.App.Services
{
    public class HubService : IHubService, IAsyncDisposable
    {
        private HubConnection? _hubConnection;
        private readonly NavigationManager _navigationManager;
        private readonly IJSRuntime _jsRuntime;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly IConfiguration _configuration;
        private readonly ILogger<HubService> _logger;
        private readonly IJwtStorageService _jwtStorageService;
        private readonly ISecureStorageService _secureStorageService; // Add secure storage

        private string? _currentHostname;
        private string? _secondaryHostname;
        private string _endpointPath = "/wss/overview"; // Default endpoint, can be changed.

        public HubService(NavigationManager navigationManager, IJSRuntime jsRuntime, AuthenticationStateProvider authenticationStateProvider, IConfiguration configuration, ILogger<HubService> logger, IJwtStorageService jwtStorageService, ISecureStorageService secureStorageService)
        {
            _navigationManager = navigationManager;
            _jsRuntime = jsRuntime;
            _authenticationStateProvider = authenticationStateProvider;
            _configuration = configuration;
            _logger = logger;
            _jwtStorageService = jwtStorageService;
            _secureStorageService = secureStorageService;
        }

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;
        public HubConnectionState State => _hubConnection?.State ?? HubConnectionState.Disconnected;

        private async Task InitializeHostnamesAsync()
        {
            if (string.IsNullOrEmpty(_currentHostname))
            {
                var (primaryHostname, secondaryHostname) = await _secureStorageService.GetHostnamesAsync();
                if (string.IsNullOrEmpty(primaryHostname) || string.IsNullOrEmpty(secondaryHostname))
                {
                    _logger.LogWarning("Primary or secondary hostnames are not configured in secure storage. Using default from config.");
                    primaryHostname = _configuration["MainHost"]; // Fallback to config if not in secure storage
                    secondaryHostname = _configuration["SecondaryHost"];
                    if (string.IsNullOrEmpty(primaryHostname))
                        primaryHostname = "http://localhost"; // ultimate fallback
                    if (string.IsNullOrEmpty(secondaryHostname))
                        secondaryHostname = "http://localhost";
                }

                _currentHostname = primaryHostname;
                _secondaryHostname = secondaryHostname;
            }
        }

        public async Task RefreshHostnamesAsync()
        {
            _logger.LogInformation("Refreshing hostnames from secure storage for HubService.");
            _currentHostname = null;
            _secondaryHostname = null;
            await InitializeHostnamesAsync();
            _logger.LogInformation("HubService Hostnames refreshed. Current hostname: {CurrentHostname}", _currentHostname);
        }

        public async Task StartAsync(string endpointPath = "/wss/overview") // Added optional endpointPath parameter
        {
            this._endpointPath = endpointPath; // Store the endpoint.
            await StartHubConnectionAsync();
        }

        private async Task StartHubConnectionAsync()
        {
            await InitializeHostnamesAsync();
            if (string.IsNullOrEmpty(_currentHostname) && string.IsNullOrEmpty(_secondaryHostname))
            {
                _logger.LogError("Both primary and secondary hostnames are not configured for SignalR.");
                return; // Exit if no hostnames are available.
            }

            if (_hubConnection != null)
            {
                if (_hubConnection.State == HubConnectionState.Connected)
                    return;
                await StopHubConnectionAsync();
                _hubConnection = null; //important to set to null so a new connection is created.
            }


            string? accessToken = await _jwtStorageService.GetTokenAsync();
            string hubUrl = $"{_currentHostname}{_endpointPath}"; // Start with primary

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

            ConfigureHubConnectionEvents(_hubConnection); //setup events

            try
            {
                await _hubConnection.StartAsync();
                _logger.LogInformation("Hub connection started at {HubUrl}", hubUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting hub connection at {HubUrl}.  Attempting Fallback", hubUrl);
                if (!string.IsNullOrEmpty(_secondaryHostname) && _currentHostname != _secondaryHostname)
                {
                    _currentHostname = _secondaryHostname; // Switch to secondary
                    hubUrl = $"{_currentHostname}{_endpointPath}";
                    _hubConnection = new HubConnectionBuilder() //rebuild
                        .WithUrl(hubUrl, options =>
                        {
                            if (!string.IsNullOrEmpty(accessToken))
                            {
                                options.AccessTokenProvider = () => Task.FromResult(accessToken);
                            }
                        })
                        .WithAutomaticReconnect()
                        .Build();
                    ConfigureHubConnectionEvents(_hubConnection);
                    try
                    {
                        await _hubConnection.StartAsync();
                        _logger.LogInformation("Hub connection started at secondary host {HubUrl}", hubUrl);
                    }
                    catch (Exception ex2)
                    {
                        _logger.LogError(ex2, "Error starting hub connection at secondary host {HubUrl}.  Both Hosts Failed", hubUrl);
                        _hubConnection = null; // set to null on total failure
                    }

                }
                else
                {
                    _logger.LogError(ex, "Error starting hub connection at {HubUrl}. Secondary host is not configured or the same as primary.  Connection Failed.", hubUrl);
                    _hubConnection = null;
                }
            }
        }
        private void ConfigureHubConnectionEvents(HubConnection connection)
        {
            connection.Reconnecting += error =>
            {
                _logger.LogWarning($"Reconnecting to hub: {error?.Message}");
                return Task.CompletedTask;
            };

            connection.Reconnected += connectionId =>
            {
                _logger.LogInformation($"Successfully reconnected to hub with connection ID: {connectionId}");
                return Task.CompletedTask;
            };

            connection.Closed += error =>
            {
                _logger.LogError($"Hub connection closed: {error?.Message}");
                // Consider attempting to restart the connection here, but with a delay and retry logic.
                Task.Run(async () => {
                    await Task.Delay(5000); // 5 second delay
                    _logger.LogInformation("Attempting to restart hub connection after closure.");
                    await StartHubConnectionAsync();
                });
                return Task.CompletedTask;
            };
        }

        private async Task StopHubConnectionAsync()
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

        public async Task StopAsync()
        {
            await StopHubConnectionAsync();
        }

        private void EnsureConnectionStarted()
        {
            if (_hubConnection == null || _hubConnection.State != HubConnectionState.Connected)
            {
                throw new InvalidOperationException("Hub connection is not started. Call StartAsync first.");
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

        public async Task InvokeAsync(string methodName, params object[] args)
        {
            EnsureConnectionStarted();
            await _hubConnection!.InvokeAsync(methodName, args);
        }

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.DisposeAsync();
                _hubConnection = null;
            }
        }
    }
}
