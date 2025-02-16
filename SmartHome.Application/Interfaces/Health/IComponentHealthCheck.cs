using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SmartHome.Dto;

namespace SmartHome.Application.Interfaces.Health
{
    public interface IComponentHealthCheck
    {
        Task<ComponentHealthCheckDto> CheckHealthAsync(CancellationToken cancellationToken = default);
    }
}
