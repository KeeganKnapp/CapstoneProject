using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Services;
using CapstoneMaui.Core.Services.Abstractions;
using CapstoneMaui.Core.Services.Auth;
using CapstoneMaui.Services;
namespace CapstoneMaui
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
				}).UseMauiMaps();

			builder.Services.AddMauiBlazorWebView();
			builder.Services.AddMudServices();

			//add API singletons here with abstraction and concrete implementation
			builder.Services.AddSingleton<IAuthService, AuthService>();
			builder.Services.AddSingleton<LocationManager>();
#if iOS
			builder.Services.AddSingleton
			<CAbstractLoggerService, Platforms.iOS.Services.iOSLoggerService>();
#endif
#if ANDROID
			builder.Services.AddSingleton
			<AbstractLoggerService, Platforms.Android.Services.AndroidLoggerService>();
#endif
#if WINDOWS
			builder.Services.AddSingleton
			<CAbstractLoggerService, Platforms.Windows.Services.WindowsLoggerService>();
#endif

#if DEBUG
		Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);
	#endif

	#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
	#endif

            return builder.Build();
		}
	}

}