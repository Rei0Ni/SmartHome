using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Dto.Dashboard
{
    public class OverviewDto
    {
        public List<OverviewAreaDto> Areas { get; set; } = new List<OverviewAreaDto>();
    }
}
