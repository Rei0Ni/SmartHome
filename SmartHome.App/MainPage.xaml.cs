using System.Net.Http;

namespace SmartHome.App
{
    public partial class MainPage : ContentPage
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public MainPage(IHttpClientFactory httpClientFactory)
        {
            InitializeComponent();
            _httpClientFactory = httpClientFactory;
        }
    }
}
