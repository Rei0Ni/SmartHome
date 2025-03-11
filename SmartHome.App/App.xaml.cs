namespace SmartHome.App
{
    public partial class App : Application
    {
        public App(IHttpClientFactory httpClientFactory, RefreshService refreshService)
        {
            InitializeComponent();

            MainPage = new MainPage(httpClientFactory, refreshService);
        }
    }
}
