using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Shared.Interfaces
{
    public interface ISecureStorageService
    {
        Task<(string?, string?)> GetHostnamesAsync();
        Task<bool> SetHostnamesAsync(string PrimaryHost, string SecondaryHost);
    }
}
