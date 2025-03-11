using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using SmartHome.Shared.Interfaces;

namespace SmartHome.App.Services
{
    public class ApiService : IApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ISecureStorageService _secureStorageService;
        private readonly IJwtStorageService _jwtStorageService; // Inject IJwtStorageService
        private readonly ILogger<ApiService> _logger;

        private string? _currentHostname;
        private string? _secondaryHostname;

        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public ApiService(
            IHttpClientFactory httpClientFactory,
            ISecureStorageService secureStorageService,
            IJwtStorageService jwtStorageService, // Receive IJwtStorageService in constructor
            ILogger<ApiService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _secureStorageService = secureStorageService;
            _jwtStorageService = jwtStorageService; // Assign IJwtStorageService
            _logger = logger;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };

            _retryPolicy = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timeSpan, retryAttempt, context) =>
                    {
                        _logger.LogWarning($"Retry #{retryAttempt} due to: {exception.Exception.Message}. Waiting {timeSpan}...");
                    }
                );
        }

        private async Task InitializeHostnamesAsync()
        {
            if (string.IsNullOrEmpty(_currentHostname))
            {
                var (primaryHostname, secondaryHostname) = await _secureStorageService.GetHostnamesAsync();
                if (string.IsNullOrEmpty(primaryHostname) || string.IsNullOrEmpty(secondaryHostname))
                {
                    _logger.LogWarning("Primary or secondary hostnames are not configured in secure storage. Using default placeholders.");
                    primaryHostname ??= "http://localhost";
                    secondaryHostname ??= "http://localhost";
                }

                _currentHostname = primaryHostname;
                _secondaryHostname = secondaryHostname;
            }
        }

        public async Task RefreshHostnamesAsync()
        {
            _logger.LogInformation("Refreshing hostnames from secure storage.");
            _currentHostname = null;
            _secondaryHostname = null;
            await InitializeHostnamesAsync();
            _logger.LogInformation("Hostnames refreshed. Current hostname: {CurrentHostname}", _currentHostname);
        }

        private HttpClient CreateHttpClient(string hostname)
        {
            var handler = new HttpClientHandler();

            // Trust SSL for the primary host (local server)
            if (hostname.StartsWith("https")) // Replace with actual local host
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
            }

            return new HttpClient(handler) { BaseAddress = new Uri(hostname) };
        }

        private async Task<HttpResponseMessage?> ExecuteRequestWithFallbackAsync(Func<HttpClient, Task<HttpResponseMessage?>> requestFunc)
        {
            await InitializeHostnamesAsync();
            if (string.IsNullOrWhiteSpace(_currentHostname) || string.IsNullOrWhiteSpace(_secondaryHostname))
            {
                _logger.LogError("Hostnames are not properly configured.");
                return default;
            }

            var primaryHttpClient = CreateHttpClient(_currentHostname); // Custom HttpClient

            // Retrieve token and add to headers if available
            var token = await _jwtStorageService.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                primaryHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            try
            {
                HttpResponseMessage? response = await _retryPolicy.ExecuteAsync(() => requestFunc(primaryHttpClient) ?? Task.FromResult<HttpResponseMessage?>(null));
                if (response != null)
                {
                    //response.EnsureSuccessStatusCode();
                    //string jsonResponse = await response.Content.ReadAsStringAsync();
                    //return JsonSerializer.Deserialize<TResponse>(jsonResponse, _jsonOptions);
                    return response;
                }

                _logger.LogError("Request returned null HttpResponseMessage unexpectedly.");
                return default;
            }
            catch (HttpRequestException exPrimary)
            {
                _logger.LogError(exPrimary, "Request to primary host ({PrimaryHost}) failed: {ErrorMessage}. Attempting secondary host.", _currentHostname, exPrimary.Message);

                if (!string.IsNullOrEmpty(_secondaryHostname) && _currentHostname != _secondaryHostname)
                {
                    _logger.LogInformation("Switching to secondary host: {SecondaryHostname}", _secondaryHostname);

                    var secondaryHttpClient = _httpClientFactory.CreateClient();
                    secondaryHttpClient.BaseAddress = new Uri(_secondaryHostname); // Secondary uses normal SSL validation

                    // Retrieve token and add to headers if available for secondary host as well (optional, depending on requirements)
                    if (!string.IsNullOrEmpty(token))
                    {
                        secondaryHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }

                    try
                    {
                        HttpResponseMessage? responseSecondary = await _retryPolicy.ExecuteAsync(() => requestFunc(secondaryHttpClient) ?? Task.FromResult<HttpResponseMessage?>(null));
                        if (responseSecondary != null)
                        {
                            //responseSecondary.EnsureSuccessStatusCode();
                            //string jsonResponseSecondary = await responseSecondary.Content.ReadAsStringAsync();
                            //return JsonSerializer.Deserialize<TResponse>(jsonResponseSecondary, _jsonOptions);
                            return responseSecondary;
                        }

                        _logger.LogError("Request to secondary host failed, but no HttpResponseMessage was returned.");
                        return default;
                    }
                    catch (HttpRequestException exSecondary)
                    {
                        _logger.LogError(exSecondary, "Request to secondary host ({SecondaryHost}) failed: {ErrorMessage}. Both hosts failed.", _secondaryHostname, exSecondary.Message);
                        return default;
                    }
                }
                else
                {
                    _logger.LogWarning("Secondary hostname not configured, same as primary, or already tried. Fallback failed.");
                    return default;
                }
            }
            catch (JsonException exJson)
            {
                _logger.LogError(exJson, "JSON Deserialization Error for host ({CurrentHost}): {ErrorMessage}", _currentHostname, exJson.Message);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected API Error for host ({CurrentHost}): {ErrorMessage}", _currentHostname, ex.Message);
                return default;
            }
        }

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
            return result!;
        }

        public async Task<HttpResponseMessage> GetAsync(string endpointPath)
        {
            var result = await ExecuteRequestWithFallbackAsync(httpClient => httpClient.GetAsync(endpointPath)! ?? Task.FromResult<HttpResponseMessage>(null)!);
            return result!;
        }

        public async Task<HttpResponseMessage> PostAsync<TRequest>(string endpointPath, TRequest requestPayload)
        {
            var result = await ExecuteRequestWithFallbackAsync(async (httpClient) =>
            {
                var response = await httpClient.PostAsJsonAsync(endpointPath, requestPayload, _jsonOptions);
                return response;

            });

            return result!;
        }
    }
}