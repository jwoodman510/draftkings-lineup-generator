using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using DraftKings.LineupGenerator.Web;
using DraftKings.LineupGenerator.Business;
using DraftKings.LineupGenerator.Razor;
using DraftKings.LineupGenerator.Caching;
using DraftKings.LineupGenerator.Razor.Services;
using Microsoft.Extensions.Http;
using DraftKings.LineupGenerator.Web.Services;
using DraftKings.LineupGenerator.Business.Logging;
using Serilog;

Log.Logger = SerilogConfiguration.Build(logToFile: false).CreateLogger();

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services
    .RegisterServices()
    .RegisterRazorServices()
    .AddScoped<ICacheService, LocalStorageCacheService>()
    .Configure<HttpClientFactoryOptions>(options =>
    {
        options.HttpMessageHandlerBuilderActions.Add(builder =>
        {
            builder.AdditionalHandlers.Add(new ProxyHttpClientHandler());
        });
    });

await builder.Build().RunAsync();
