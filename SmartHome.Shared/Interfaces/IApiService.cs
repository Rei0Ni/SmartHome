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
        Task RefreshHostnamesAsync();
        Task<HttpResponseMessage> SendAsync<TRequest>(HttpMethod method, string endpointPath, TRequest requestPayload = default);
        Task<HttpResponseMessage> GetAsync(string endpointPath);
        Task<HttpResponseMessage> PostAsync<TRequest>(string endpointPath, TRequest requestPayload);
    }
}
