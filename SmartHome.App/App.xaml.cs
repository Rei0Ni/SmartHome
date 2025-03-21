using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Serilog;
using Application = Microsoft.Maui.Controls.Application;

namespace SmartHome.App
{
    public partial class App : Application
    {
        public App(IHttpClientFactory httpClientFactory, RefreshService refreshService)
        {
            InitializeComponent();

            MainPage = new MainPage(httpClientFactory, refreshService);

            Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);

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
