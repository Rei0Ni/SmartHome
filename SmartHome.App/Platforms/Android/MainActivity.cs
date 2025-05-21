using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.AppCompat.App;

namespace SmartHome.App
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            // Force the app to use a fixed theme (ignores system theme)
            AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo; // Light theme
                                                                                // AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightYes; // Dark theme

            base.OnCreate(savedInstanceState);
        }
    }
}
