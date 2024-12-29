using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SmartHome.Application.DTOs
{
    public class HealthCheckDto
    {
        public required string Status { get; set; }
        public List<ComponentHealthCheckDto>? Checks { get; set; }
    }
}
