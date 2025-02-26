using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Dto.Dashboard;
using SmartHome.Shared.Interfaces;

namespace SmartHome.Shared.Pages
{
    public partial class Overview
    {
        public OverviewDto OverviewData { get; set; } = new();
        protected override async Task OnInitializedAsync()
        {
            var result = await ApiService.GetAsync("/api/dashboard/overview");

            var response = await result.Content.ReadFromJsonAsync<OverviewDto>();

            OverviewData = response;
        }
    }
}
