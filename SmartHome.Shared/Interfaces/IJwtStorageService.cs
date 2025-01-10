using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Shared.Interfaces
{
    public interface IJwtStorageService
    {
        Task SaveTokenAsync(string session);
        Task<string?> GetTokenAsync();
        Task RemoveTokenAsync();
    }
}
