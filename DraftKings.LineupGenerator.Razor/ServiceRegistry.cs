using Blazored.LocalStorage;
using Microsoft.Extensions.DependencyInjection;

namespace DraftKings.LineupGenerator.Razor
{
    public static class ServiceRegistry
    {
        public static IServiceCollection RegisterRazorServices(this IServiceCollection services)
        {
            return services
                .AddBlazorBootstrap()
                .AddBlazoredLocalStorage();
        }
    }
}
