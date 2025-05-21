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
        public OverviewDeviceDto TempratureSensor { get; set; } = new();
        public string CurrentDate { get; set; }

        private bool isLoading = false;
        protected override async Task OnInitializedAsync()
        {
            isLoading = true;

            await SettingsService.LoadSettingsAsync();

            var SystemTimeZone = await SettingsService.GetSystemTimeZone();
            UpdateCurrentDate(SystemTimeZone); // Initialize the date

            // Start a timer to update the date every minute
            var timer = new System.Timers.Timer(10000); // 60 seconds
            timer.Elapsed += (sender, e) => UpdateCurrentDate(SystemTimeZone);
            timer.AutoReset = true;
            timer.Start();

            var result = await ApiService.GetAsync("/api/dashboard/overview");

            var response = await result.Content.ReadFromJsonAsync<OverviewDto>();

            OverviewData = response;

            await HubService.StartAsync("wss/overview");

            HubService.On<OverviewDto>("ReceiveOverviewData", async (data) =>
            {
                Console.WriteLine("Overview Data Recieved");
                OverviewData = data;
                foreach (var area in data.Areas)
                {
                    foreach (var device in area.AreaDevices)
                    {
                        if (device.DeviceType.Type == Enum.DeviceTypes.TEMPRATURE_SENSOR)
                        {
                            TempratureSensor = device;
                        }
                    }
                }
                await InvokeAsync(StateHasChanged);
            });

            refreshService.OnRefreshRequested += HandleRefreshRequested;

            foreach (var area in OverviewData!.Areas)
            {
                foreach (var device in area.AreaDevices)
                {
                    if (device.DeviceType.Type == Enum.DeviceTypes.TEMPRATURE_SENSOR)
                    {
                        TempratureSensor = device;
                    }
                }
            }

            isLoading = false;
        }

        private void UpdateCurrentDate(TimeZoneInfo timeZone)
        {
            CurrentDate = TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZone).ToString("f");
            InvokeAsync(StateHasChanged); // Ensure UI updates
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
