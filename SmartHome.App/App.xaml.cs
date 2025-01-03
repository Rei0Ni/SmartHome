namespace SmartHome.App
{
    public partial class App : Application
    {
        public App(IHttpClientFactory httpClientFactory)
        {
            InitializeComponent();

            MainPage = new MainPage(httpClientFactory);
        }
    }
}
