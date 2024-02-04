using Blazored.LocalStorage;
using Blazored.LocalStorage.Serialization;
using DraftKings.LineupGenerator.Razor.Contests;
using DraftKings.LineupGenerator.Razor.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DraftKings.LineupGenerator.Razor
{
    public static class ServiceRegistry
    {
        public static IServiceCollection RegisterRazorServices(this IServiceCollection services)
        {
            return services
                .AddLocalStorage()
                .AddBlazorBootstrap()
                .AddScoped<ContestStateProvider>()
                .AddTransient<IRecentContestService, RecentContestService>()
                .AddTransient<IContestHistoryService, ContestHistoryService>();
        }

        private static IServiceCollection AddLocalStorage(this IServiceCollection services)
        {
            services.AddBlazoredLocalStorage();
            services.Replace(ServiceDescriptor.Scoped<IJsonSerializer, LocalStorageSerializer>());

            return services;
        }
    }
}
