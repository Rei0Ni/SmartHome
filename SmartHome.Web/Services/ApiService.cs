using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using SmartHome.Shared.Interfaces;

namespace SmartHome.Web.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<ApiService> _logger;
        private readonly IJwtStorageService _jwtStorageService;

        public ApiService(IHttpClientFactory httpClientFactory, ILogger<ApiService> logger, IJwtStorageService jwtStorageService)
        {
            _httpClient = httpClientFactory.CreateClient("AuthClient") ?? throw new ArgumentNullException(nameof(_httpClient));
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
            _logger = logger;
            _jwtStorageService = jwtStorageService;
        }

        private async Task<HttpResponseMessage?> ExecuteRequestWithFallbackAsync(Func<HttpClient, Task<HttpResponseMessage?>> requestFunc)
        {
            // Retrieve token and add to headers if available
            var token = await _jwtStorageService.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            try
            {
                HttpResponseMessage? response = await requestFunc(_httpClient) ?? await Task.FromResult<HttpResponseMessage?>(null);
                if (response != null)
                {
                    return response;
                }

                _logger.LogError("Request returned null HttpResponseMessage unexpectedly.");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected API Error for host ({Host}): {ErrorMessage}", _httpClient.BaseAddress, ex.Message);
                return default;
            }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<HttpResponseMessage> GetAsync(string endpointPath)
        {
            return await ExecuteRequestWithFallbackAsync(httpClient => httpClient.GetAsync(endpointPath) ?? Task.FromResult<HttpResponseMessage?>(null));
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> PostAsync<TRequest>(string endpointPath, TRequest requestPayload)
        {
            var response = await ExecuteRequestWithFallbackAsync(async (httpClient) =>
            {
                return await httpClient.PostAsJsonAsync(endpointPath, requestPayload, _jsonOptions);
            });

            return response!;
        }

        // --- NEW METHOD FOR SENDING ARBITRARY HTTPCONTENT (like MultipartFormDataContent) ---
        /// <summary>
        /// Sends a POST request with arbitrary HttpContent (e.g., MultipartFormDataContent).
        /// </summary>
        /// <param name="endpointPath">The API endpoint path.</param>
        /// <param name="content">The HttpContent to send (e.g., MultipartFormDataContent).</param>
        /// <returns>The HttpResponseMessage.</returns>
        public async Task<HttpResponseMessage> PostAsync(string endpointPath, MultipartFormDataContent content)
        {
            var result = await ExecuteRequestWithFallbackAsync(async (httpClient) =>
            {
                // Use the standard PostAsync overload that takes HttpContent
                return await httpClient.PostAsync(endpointPath, content);
            });

            // Ensure result is not null before returning
            return result ?? new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
        }

        public Task RefreshHostnamesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetCurrentHost()
        {
            return _httpClient.BaseAddress.Host;
        }
    }
}
