using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using SmartHome.App.Services;
using SmartHome.Shared.Interfaces;
using SmartHome.Shared.Providers;
using SmartHome.Shared.Services;
using Serilog;
using Serilog.Sinks.File;
using Serilog.Extensions.Logging;
using TailBlazor.Toast;
using Blazored.Toast;

namespace SmartHome.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Configure Serilog
            ConfigureLogging(builder.Logging);

            builder.Services.AddMauiBlazorWebView();

            // Add device-specific services used by the MauiApp1.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IJwtStorageService, JwtStorageService>();
            builder.Services.AddScoped<JwtAuthStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<JwtAuthStateProvider>());

            builder.Services.AddSingleton<IFormFactor, FormFactor>();
            builder.Services.AddSingleton<IApiService, ApiService>();
            builder.Services.AddScoped<IHubService, HubService>();
            builder.Services.AddScoped<ISecureStorageService, SecureStorageService>();
            builder.Services.AddScoped<IPlatformDetectionService, PlatformDetectionService>();
            builder.Services.AddScoped<IHostConfigurationCheckService, HostConfigurationCheckService>();
            builder.Services.AddScoped<IThemeService, ThemeService>();


            builder.Services.AddSingleton<IHostStatusService, HostStatusService>();
            builder.Services.AddSingleton<ISettingsService, SettingsService>();
            builder.Services.AddSingleton<INetworkMonitor, NetworkMonitor>();
            builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);

            //builder.Services.AddBlazorBootstrap();
            //builder.Services.AddTailBlazorToast();
            builder.Services.AddBlazoredToast();

            // Configure global JSON options
            builder.Services.Configure<JsonSerializerOptions>(options =>
            {
                options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                //options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; // Ignore null values
                options.WriteIndented = true;
            });

            builder.Services.AddHttpClient("AuthClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7019/");
            });

            builder.Services.AddAuthorizationCore();
            builder.Services.AddSingleton<RefreshService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
            
#endif
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine(e.ExceptionObject);
            };

            AppDomain.CurrentDomain.FirstChanceException += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine("********** ERROR!!! FirstChanceException **********");
                System.Diagnostics.Debug.WriteLine(e.Exception.ToString());
            };

            return builder.Build();
        }

        private static void ConfigureLogging(ILoggingBuilder loggingBuilder)
        {
            string logFilePath = Path.Combine(FileSystem.AppDataDirectory, "MyAppLogs.txt");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(
                    logFilePath,
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            loggingBuilder.AddSerilog(Log.Logger, dispose: true);
        }
    }
}
