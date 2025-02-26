using System.Net.Http;
using System.Text.Json;
using SmartHome.Shared.Interfaces;

namespace SmartHome.Web.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("AuthClient") ?? throw new ArgumentNullException(nameof(_httpClient));
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }

        /// <inheritdoc />
        public async Task<TResponse> SendAsync<TResponse, TRequest>(HttpMethod method, string endpointPath, TRequest requestPayload = default)
        {
            try
            {
                HttpResponseMessage response = null;

                if (method == HttpMethod.Get)
                {
                    response = await _httpClient.GetAsync(endpointPath);
                }
                else if (method == HttpMethod.Post)
                {
                    response = await _httpClient.PostAsJsonAsync(endpointPath, requestPayload, _jsonOptions);
                }
                else if (method == HttpMethod.Put)
                {
                    response = await _httpClient.PutAsJsonAsync(endpointPath, requestPayload, _jsonOptions);
                }
                else if (method == HttpMethod.Patch)
                {
                    response = await _httpClient.PatchAsync(endpointPath, JsonContent.Create(requestPayload, options: _jsonOptions));
                }
                else if (method == HttpMethod.Delete)
                {
                    response = await _httpClient.DeleteAsync(endpointPath);
                }
                else
                {
                    throw new ArgumentException($"Unsupported HTTP method: {method}", nameof(method));
                }

                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                TResponse responseObject = JsonSerializer.Deserialize<TResponse>(jsonResponse, _jsonOptions);
                return responseObject;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Request Error ({method} {endpointPath}): {ex.Message}");
                return default;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Deserialization Error ({method} {endpointPath}): {ex.Message}");
                Console.WriteLine($"Raw JSON Response (for debugging):\n{await _httpClient.GetStringAsync(endpointPath)}");
                return default;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected API Error ({method} {endpointPath}): {ex.Message}");
                return default;
            }
        }

        /// <inheritdoc />
        public async Task<TResponse> GetAsync<TResponse>(string endpointPath)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<TResponse>(endpointPath, _jsonOptions);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP GET Request Error ({endpointPath}): {ex.Message}");
                return default;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Deserialization Error (GET {endpointPath}): {ex.Message}");
                Console.WriteLine($"Raw JSON Response (for debugging):\n{await _httpClient.GetStringAsync(endpointPath)}");
                return default;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected API Error (GET {endpointPath}): {ex.Message}");
                return default;
            }
        }

        /// <inheritdoc />
        public async Task<TResponse> PostAsync<TResponse, TRequest>(string endpointPath, TRequest requestPayload)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpointPath, requestPayload, _jsonOptions);
                response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();
                TResponse responseObject = JsonSerializer.Deserialize<TResponse>(jsonResponse, _jsonOptions);
                return responseObject;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP POST Request Error ({endpointPath}): {ex.Message}");
                return default;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Deserialization Error (POST {endpointPath}): {ex.Message}");
                return default;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected API Error (POST {endpointPath}): {ex.Message}");
                return default;
            }
        }
    }
}
