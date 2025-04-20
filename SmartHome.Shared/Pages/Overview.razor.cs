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
        public string CurrentDate { get; set; }
        protected override async Task OnInitializedAsync()
        {
            CurrentDate = DateTime.Now.ToString("D");

            var result = await ApiService.GetAsync("/api/dashboard/overview");

            var response = await result.Content.ReadFromJsonAsync<OverviewDto>();

            OverviewData = response;

            await HubService.StartAsync("/wss/overview");

            HubService.On<OverviewDto>("ReceiveOverviewData", async (data) =>
            {
                Console.WriteLine("Overview Data Recieved");
                OverviewData = data;
                foreach (var area in data.Areas)
                {
                    foreach (var device in area.AreaDevices)
                    {
                        if (device.DeviceType.Type == Enum.DeviceTypes.PIR_MOTION_SENSOR)
                        {
                            Console.WriteLine(device.State.First().Key);
                            Console.WriteLine(device.State.First().Value);
                        }
                    }
                }
                await InvokeAsync(StateHasChanged);
            });

            refreshService.OnRefreshRequested += HandleRefreshRequested;
        }

        private void HandleRefreshRequested()
        {
            // Reload the page using NavigationManager.
            navigationManager.NavigateTo(navigationManager.Uri, forceLoad: true);
        }

        private async void logout()
        {
            await authStateProvider.Logout();
        }

        public void Dispose()
        {
            // Unsubscribe to avoid memory leaks.
            refreshService.OnRefreshRequested -= HandleRefreshRequested;
        }
    }


}
