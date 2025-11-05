using Android.App;
using Android.Content.PM;
using Android.OS;

namespace CapstoneMaui;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        const int RequestId = 1000;
string[] Permissions = new[]
{
    Android.Manifest.Permission.AccessFineLocation,
    Android.Manifest.Permission.AccessCoarseLocation
};

if (AndroidX.Core.Content.ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessFineLocation)
    != Android.Content.PM.Permission.Granted)
{
    AndroidX.Core.App.ActivityCompat.RequestPermissions(this, Permissions, RequestId);
}
    }
}
