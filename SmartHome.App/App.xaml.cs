using Serilog;

namespace SmartHome.App
{
    public partial class App : Application
    {
        public App(IHttpClientFactory httpClientFactory, RefreshService refreshService)
        {
            InitializeComponent();

            MainPage = new MainPage(httpClientFactory, refreshService);

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception)e.ExceptionObject;
            Log.Logger.Error(exception, "Unhandled .NET Exception");
        }

        private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Log.Logger.Error(e.Exception, "Unobserved Task Exception");
            e.SetObserved(); // Mark the exception as observed to prevent app termination (optional)
        }
    }
}
