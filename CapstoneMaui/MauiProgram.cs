using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Services;
using CapstoneMaui.Core.Services.Abstractions;
using CapstoneMaui.Core.Services.Auth;
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
				});

			builder.Services.AddMauiBlazorWebView();
			builder.Services.AddMudServices();

			//add API singletons here with abstraction and concrete implementation
			builder.Services.AddSingleton<IAuthService, AuthService>();

	#if ANDROID && DEBUG
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