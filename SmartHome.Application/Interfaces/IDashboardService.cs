using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Dto.Dashboard;

namespace SmartHome.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<OverviewDto> GetDashboardOverview();
    }
}
