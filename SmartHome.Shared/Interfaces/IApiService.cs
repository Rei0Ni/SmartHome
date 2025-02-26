using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Shared.Interfaces
{
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface IApiService
    {
        /// <summary>
        /// Sends an HTTP request of the specified method to the API endpoint and returns a typed response.
        /// </summary>
        /// <typeparam name="TResponse">The type of the expected response DTO.</typeparam>
        /// <typeparam name="TRequest">The type of the request payload DTO.</typeparam>
        /// <param name="method">The HTTP method (GET, POST, PUT, DELETE, PATCH).</param>
        /// <param name="endpointPath">The relative path to the API endpoint.</param>
        /// <param name="requestPayload">The request payload DTO (optional for GET requests).</param>
        /// <returns>A task that represents the asynchronous operation, returning the deserialized response DTO of type TResponse, or default(TResponse) in case of error.</returns>
        Task<TResponse> SendAsync<TResponse, TRequest>(HttpMethod method, string endpointPath, TRequest requestPayload = default);

        /// <summary>
        /// Sends an HTTP GET request to the API endpoint and returns a typed response.
        /// </summary>
        /// <typeparam name="TResponse">The type of the expected response DTO.</typeparam>
        /// <param name="endpointPath">The relative path to the API endpoint.</param>
        /// <returns>A task that represents the asynchronous operation, returning the deserialized response DTO of type TResponse, or default(TResponse) in case of error.</returns>
        Task<TResponse> GetAsync<TResponse>(string endpointPath);

        /// <summary>
        /// Sends an HTTP POST request to the API endpoint with a request payload and returns a typed response.
        /// </summary>
        /// <typeparam name="TResponse">The type of the expected response DTO.</typeparam>
        /// <typeparam name="TRequest">The type of the request payload DTO.</typeparam>
        /// <param name="endpointPath">The relative path to the API endpoint.</param>
        /// <param name="requestPayload">The request payload DTO.</param>
        /// <returns>A task that represents the asynchronous operation, returning the deserialized response DTO of type TResponse, or default(TResponse) in case of error.</returns>
        Task<TResponse> PostAsync<TResponse, TRequest>(string endpointPath, TRequest requestPayload);

        // You can add interfaces for other HTTP methods like PutAsync, DeleteAsync, PatchAsync if needed.
    }
}
