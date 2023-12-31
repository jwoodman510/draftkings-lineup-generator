using Blazored.LocalStorage;
using DraftKings.LineupGenerator.Razor.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DraftKings.LineupGenerator.Razor
{
    public static class ServiceRegistry
    {
        public static IServiceCollection RegisterRazorServices(this IServiceCollection services)
        {
            return services
                .AddBlazorBootstrap()
                .AddBlazoredLocalStorage()
                .AddTransient<IContestService, ContestService>();
        }
    }
}
