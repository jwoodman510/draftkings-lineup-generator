using DraftKings.LineupGenerator.Business;
using DraftKings.LineupGenerator.Business.Logging;
using DraftKings.LineupGenerator.Razor;
using Microsoft.Extensions.Logging;
using Serilog;

namespace DraftKings.LineupGenerator.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            Log.Logger = SerilogConfiguration.Build().CreateLogger();

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            builder.Services
                .RegisterServices()
                .RegisterRazorServices();

            return builder.Build();
        }
    }
}
