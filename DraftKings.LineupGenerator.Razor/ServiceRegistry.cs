using Blazored.LocalStorage;
using DraftKings.LineupGenerator.Razor.Services;
using DraftKings.LineupGenerator.Razor.State;
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
                .AddSingleton<ContestStateProvider>()
                .AddTransient<IRecentContestService, RecentContestService>();
        }
    }
}
