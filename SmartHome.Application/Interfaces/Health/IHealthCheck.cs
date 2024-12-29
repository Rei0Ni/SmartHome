using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Application.DTOs;

namespace SmartHome.Application.Interfaces.Health
{
    public interface IHealthCheck
    {
        Task<HealthCheckDto> CheckHealthAsync(CancellationToken cancellationToken = default);
    }
}
