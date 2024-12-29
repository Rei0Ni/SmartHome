using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SmartHome.Application.DTOs
{
    public class ComponentHealthCheckDto
    {
        public required string Component { get; set; }
        public required string Status { get; set; }
        public Dictionary<string, object>? Details { get; set; }
    }
}
