using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SmartHome.Application.Delegates;
using SmartHome.Dto;
using SmartHome.Application.Enums;
using SmartHome.Application.Interfaces.Health;
using HealthStatus = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus;
using IHealthCheck = SmartHome.Application.Interfaces.Health.IHealthCheck;

namespace SmartHome.Application.Health_Checks
{
    public class SystemHealthCheck : IHealthCheck
    {
        public readonly IComponentHealthCheck _mongodbHealthCheck;
        public readonly IComponentHealthCheck _jwtTokenServiceHealthCheck;
        public SystemHealthCheck(ServiceResolver<IComponentHealthCheck> serviceResolver)
        {
            _mongodbHealthCheck = serviceResolver(ComponentHealthChecks.MongodbHealthCheck);
            _jwtTokenServiceHealthCheck = serviceResolver(ComponentHealthChecks.JwtTokenServiceHealthCheck);
        }

        public async Task<HealthCheckDto> CheckHealthAsync(CancellationToken cancellationToken = default)
        {
            List<ComponentHealthCheckDto> componentHealthChecks = new List<ComponentHealthCheckDto>();

            componentHealthChecks.Add(await _mongodbHealthCheck.CheckHealthAsync(cancellationToken));
            componentHealthChecks.Add(await _jwtTokenServiceHealthCheck.CheckHealthAsync(cancellationToken));

            var systemHealth = new HealthCheckDto()
            {
                Status = DetermineMostSevereStatus(componentHealthChecks).ToString(),
                Checks = componentHealthChecks
            };
            return systemHealth;
        }

        private HealthStatus DetermineMostSevereStatus(List<ComponentHealthCheckDto> componentHealthChecks)
        {
            var statuses = componentHealthChecks.Select(c => Enum.Parse<HealthStatus>(c.Status)).ToList();

            // Return the most severe status (Unhealthy > Degraded > Healthy)
            if (statuses.Contains(HealthStatus.Unhealthy))
                return HealthStatus.Unhealthy;
            if (statuses.Contains(HealthStatus.Degraded))
                return HealthStatus.Degraded;

            return HealthStatus.Healthy;
        }
    }
}
