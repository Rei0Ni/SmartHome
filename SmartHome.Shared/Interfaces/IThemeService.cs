using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Shared.Interfaces
{
    public interface IThemeService
    {
        event Action? OnThemeChanged;
        Task<string?> GetThemeAsync();
        Task SetThemeAsync(string theme);
    }
}
