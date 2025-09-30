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

			builder.Services.AddSingleton<IAuthService, AuthService>();

#if DEBUG
		Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

			return builder.Build();
		}
	}

}