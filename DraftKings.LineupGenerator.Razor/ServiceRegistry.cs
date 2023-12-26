using Microsoft.Extensions.DependencyInjection;

namespace DraftKings.LineupGenerator.Razor
{
    public static class ServiceRegistry
    {
        public static IServiceCollection RegisterRazorServices(this IServiceCollection services)
        {
            services.AddBlazorBootstrap();

            return services;
        }
    }
}
