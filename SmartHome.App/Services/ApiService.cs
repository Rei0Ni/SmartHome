using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using SmartHome.Shared.Interfaces; // For IApiService, IHostStatusService, ISecureStorageService, IJwtStorageService

namespace SmartHome.App.Services
{
    /// <summary>
    /// Provides API communication services with fallback mechanisms and error signaling.
    /// Hostnames are fetched from secure storage on every API request to ensure they are always fresh.
    /// </summary>
    public class ApiService : IApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ISecureStorageService _secureStorageService; // Used to fetch hostnames
        private readonly IJwtStorageService _jwtStorageService;
        private readonly IHostStatusService _hostStatusService;
        private readonly INetworkMonitor _networkMonitor; // Assuming this is part of your setup
        private readonly ILogger<ApiService> _logger;

        // Hostnames are no longer cached as fields; they will be fetched per request.
        // private string? _primaryHostname;
        // private string? _secondaryHostname;

        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public ApiService(
            IHttpClientFactory httpClientFactory,
            ISecureStorageService secureStorageService,
            IJwtStorageService jwtStorageService,
            ILogger<ApiService> logger,
            IHostStatusService hostStatusService,
            INetworkMonitor networkMonitor) // Assuming INetworkMonitor is injected
        {
            _httpClientFactory = httpClientFactory;
            _secureStorageService = secureStorageService; // Keep this for fetching hostnames
            _jwtStorageService = jwtStorageService;
            _logger = logger;
            _hostStatusService = hostStatusService;
            _networkMonitor = networkMonitor; // Assign INetworkMonitor

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };

            // Retry policy configuration (same as before)
            _retryPolicy = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(
                    retryCount: 0, // Adjust as needed.
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(1),
                    onRetry: (exception, timeSpan, retryAttempt, context) =>
                    {
                        _logger.LogWarning($"Retry #{retryAttempt} due to: {exception.Exception.Message}. Waiting {timeSpan}...");
                    }
                );
        }

        // InitializeHostnamesAsync is no longer needed as hostnames are fetched per request.
        // public async Task InitializeHostnamesAsync() { ... }

        // RefreshHostnamesAsync is no longer needed as hostnames are fetched per request.
        // public async Task RefreshHostnamesAsync() { ... }

        private HttpClient CreateHttpClient(string hostname)
        {
            return new HttpClient() { BaseAddress = new Uri(hostname) };
        }

        /// <summary>
        /// Executes an HTTP request with a fallback mechanism to a secondary host if the primary fails.
        /// Hostnames are fetched from secure storage at the start of this method to ensure freshness.
        /// Signals host configuration errors via IHostStatusService and always returns a non-null HttpResponseMessage.
        /// </summary>
        /// <param name="requestFunc">A function that takes an HttpClient and returns the HttpResponseMessage.</param>
        /// <returns>The HttpResponseMessage from the successful request, or an error response if all hosts fail.</returns>
        private async Task<HttpResponseMessage> ExecuteRequestWithFallbackAsync(Func<HttpClient, Task<HttpResponseMessage?>> requestFunc)
        {
            // 1. Check Network Status
            _networkMonitor.ForceUpdateStatus(); // Ensure network status is current

            if (_networkMonitor.CurrentStatus != NetworkStatus.Internet)
            {
                _logger.LogError("No Internet Connection. Signalling network error.");
                _hostStatusService.SetNetworkConnectionError(true); // Signal network connection error
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    ReasonPhrase = "No Internet Connection"
                };
            }
            else
            {
                _hostStatusService.SetNetworkConnectionError(false); // Clear network error if connected
            }

            // 2. Fetch Hostnames from Secure Storage on every request
            var (primaryHostname, secondaryHostname) = await _secureStorageService.GetHostnamesAsync();

            // Handle case where hostnames are not properly configured in storage
            if (string.IsNullOrWhiteSpace(primaryHostname) || string.IsNullOrWhiteSpace(secondaryHostname))
            {
                _logger.LogError("Hostnames are not properly configured in secure storage. Signalling host error.");
                _hostStatusService.SetHostConfigurationError(true); // Signal the host configuration error
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    ReasonPhrase = "Hostnames not configured."
                };
            }

            var currentPrimaryHostname = primaryHostname; // Use local variables for clarity
            var currentSecondaryHostname = secondaryHostname;

            var primaryHttpClient = CreateHttpClient(currentPrimaryHostname);

            // Retrieve token and add to headers if available
            var token = await _jwtStorageService.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                primaryHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            try
            {
                // Attempt request to primary host with retry policy.
                HttpResponseMessage? response = await _retryPolicy.ExecuteAsync(() => requestFunc(primaryHttpClient) ?? Task.FromResult<HttpResponseMessage?>(null));
                if (response != null)
                {
                    // If the response is successful, clear any host error state
                    if (response.IsSuccessStatusCode)
                    {
                        _hostStatusService.SetHostConfigurationError(false); // Clear host config error
                    }
                    return response;
                }

                _logger.LogError("Request returned null HttpResponseMessage unexpectedly from primary host.");
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    ReasonPhrase = "Unexpected null response from primary host."
                };
            }
            catch (HttpRequestException exPrimary)
            {
                _logger.LogError(exPrimary, "Request to primary host ({PrimaryHost}) failed: {ErrorMessage}. Attempting secondary host.", currentPrimaryHostname, exPrimary.Message);

                // Attempt fallback to secondary host
                if (!string.IsNullOrEmpty(currentSecondaryHostname) && currentPrimaryHostname != currentSecondaryHostname)
                {
                    _logger.LogInformation("Switching to secondary host: {SecondaryHostname}", currentSecondaryHostname);

                    var secondaryHttpClient = _httpClientFactory.CreateClient();
                    secondaryHttpClient.BaseAddress = new Uri(currentSecondaryHostname);

                    if (!string.IsNullOrEmpty(token))
                    {
                        secondaryHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }

                    try
                    {
                        HttpResponseMessage? responseSecondary = await _retryPolicy.ExecuteAsync(() => requestFunc(secondaryHttpClient) ?? Task.FromResult<HttpResponseMessage?>(null));
                        if (responseSecondary != null)
                        {
                            if (responseSecondary.IsSuccessStatusCode)
                            {
                                _hostStatusService.SetHostConfigurationError(false); // Clear host config error if successful
                            }
                            return responseSecondary;
                        }

                        _logger.LogError("Request to secondary host failed, but no HttpResponseMessage was returned.");
                        return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                        {
                            ReasonPhrase = "Unexpected null response from secondary host."
                        };
                    }
                    catch (HttpRequestException exSecondary)
                    {
                        _logger.LogError(exSecondary, "Request to secondary host ({SecondaryHost}) failed: {ErrorMessage}. Both hosts failed.", currentSecondaryHostname, exSecondary.Message);
                        _hostStatusService.SetHostConfigurationError(true); // Signal error as both hosts failed
                        return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                        {
                            ReasonPhrase = "Both primary and secondary hosts failed."
                        };
                    }
                }
                else // No secondary host, or secondary is same as primary, or already tried.
                {
                    _logger.LogWarning("Secondary hostname not configured, same as primary, or already tried. Fallback failed.");
                    _hostStatusService.SetHostConfigurationError(true); // Signal error as fallback failed
                    return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                    {
                        ReasonPhrase = "Primary host failed, no valid secondary host or fallback failed."
                    };
                }
            }
            catch (JsonException exJson)
            {
                _logger.LogError(exJson, "JSON Deserialization Error for host ({CurrentHost}): {ErrorMessage}", currentPrimaryHostname, exJson.Message);
                _hostStatusService.SetHostConfigurationError(true); // Signal error for deserialization issues too
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = "JSON deserialization error."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected API Error for host ({CurrentHost}): {ErrorMessage}", currentPrimaryHostname, ex.Message);
                _hostStatusService.SetHostConfigurationError(true); // Signal error for unexpected exceptions
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    ReasonPhrase = "An unexpected API error occurred."
                };
            }
        }

        // Public methods remain the same, as they call ExecuteRequestWithFallbackAsync
        public async Task<HttpResponseMessage> SendAsync<TRequest>(HttpMethod method, string endpointPath, TRequest requestPayload = default)
        {
            var result = await ExecuteRequestWithFallbackAsync(async (httpClient) =>
            {
                return method switch
                {
                    { } when method == HttpMethod.Get => await httpClient.GetAsync(endpointPath),
                    { } when method == HttpMethod.Post => await httpClient.PostAsJsonAsync(endpointPath, requestPayload, _jsonOptions),
                    { } when method == HttpMethod.Put => await httpClient.PutAsJsonAsync(endpointPath, requestPayload, _jsonOptions),
                    { } when method == HttpMethod.Patch => await httpClient.PatchAsync(endpointPath, JsonContent.Create(requestPayload, options: _jsonOptions)),
                    { } when method == HttpMethod.Delete => await httpClient.DeleteAsync(endpointPath),
                    _ => throw new ArgumentException($"Unsupported HTTP method: {method}", nameof(method))
                };
            });
            return result;
        }

        public async Task<HttpResponseMessage> GetAsync(string endpointPath)
        {
            var result = await ExecuteRequestWithFallbackAsync(httpClient => httpClient.GetAsync(endpointPath));
            return result;
        }

        public async Task<HttpResponseMessage> PostAsync<TRequest>(string endpointPath, TRequest requestPayload)
        {
            var result = await ExecuteRequestWithFallbackAsync(async (httpClient) =>
            {
                var response = await httpClient.PostAsJsonAsync(endpointPath, requestPayload, _jsonOptions);
                return response;
            });
            return result;
        }

        public async Task<HttpResponseMessage> PostAsync(string endpointPath, MultipartFormDataContent content)
        {
            var result = await ExecuteRequestWithFallbackAsync(async (httpClient) =>
            {
                var response = await httpClient.PostAsync(endpointPath, content);
                return response;
            });
            return result;
        }

        public Task RefreshHostnamesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetCurrentHost()
        {
            var (primaryHostname, secondaryHostname) = await _secureStorageService.GetHostnamesAsync();

            // Handle case where hostnames are not properly configured in storage
            if (string.IsNullOrWhiteSpace(primaryHostname) || string.IsNullOrWhiteSpace(secondaryHostname))
            {
                _logger.LogError("Hostnames are not properly configured in secure storage. Signalling host error.");
                _hostStatusService.SetHostConfigurationError(true); // Signal the host configuration error
                return null;
            }

            var token = await _jwtStorageService.GetTokenAsync();

            // Helper function to check if a host is reachable
            async Task<bool> IsHostWorkingAsync(string hostname)
            {
                try
                {
                    using var httpClient = CreateHttpClient(hostname);
                    if (!string.IsNullOrEmpty(token))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }
                    var response = await httpClient.GetAsync("/api/health");
                    return response.IsSuccessStatusCode;
                }
                catch
                {
                    return false;
                }
            }

            if (await IsHostWorkingAsync(primaryHostname))
            {
                return primaryHostname;
            }
            if (await IsHostWorkingAsync(secondaryHostname))
            {
                return secondaryHostname;
            }

            _logger.LogError("Neither primary nor secondary host is reachable.");
            _hostStatusService.SetHostConfigurationError(true);
            return null;
        }
    }
}
