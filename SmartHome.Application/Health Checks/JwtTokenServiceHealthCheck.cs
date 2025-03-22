using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SmartHome.Dto;
using SmartHome.Application.Interfaces.Health;
using SmartHome.Application.Interfaces.Jwt;

namespace SmartHome.Application.Health_Checks
{
    public class JwtTokenServiceHealthCheck : IComponentHealthCheck
    {
        private readonly IJwtTokenService _jwtTokenService;

        public JwtTokenServiceHealthCheck(IJwtTokenService jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
        }

        public async Task<ComponentHealthCheckDto> CheckHealthAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var token = _jwtTokenService.GenerateJwtToken("033ff606-a778-41a5-aa48-fc97437bc59b", "test", "test@test.com", null, ["ADMIN"]);
                _jwtTokenService.ValidateJwtToken(token);

                return new ComponentHealthCheckDto()
                {
                    Component = "JwtTokenService",
                    Status = HealthStatus.Healthy.ToString(),
                    Details = new Dictionary<string, object> { { "ok", "Generated Token is Valid" } }
                };
            }
            catch (Exception)
            {

                return new ComponentHealthCheckDto()
                {
                    Component = "JwtTokenService",
                    Status = HealthStatus.Unhealthy.ToString(),
                    Details = new Dictionary<string, object> { { "Err", "Generated Token is Invalid" } }
                };
            }
        }
    }
}
