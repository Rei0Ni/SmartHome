using System.Net.Http;

namespace SmartHome.App
{
    public partial class MainPage : ContentPage
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly RefreshService _refreshService;

        public MainPage(IHttpClientFactory httpClientFactory, RefreshService refreshService)
        {
            InitializeComponent();
            _httpClientFactory = httpClientFactory;
            _refreshService = refreshService;
        }

        private void RefreshContainer_Refreshing(object sender, EventArgs e)
        {
            // Signal the Blazor component to refresh.
            _refreshService.RequestRefresh();

            // Stop the RefreshView indicator.
            RefreshContainer.IsRefreshing = false;
        }
    }
}
