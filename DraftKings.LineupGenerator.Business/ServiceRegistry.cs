using DraftKings.LineupGenerator.Api;
using DraftKings.LineupGenerator.Api.Draftables;
using DraftKings.LineupGenerator.Api.Rules;
using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Business.LineupGenerators.SalaryCap.Classic;
using DraftKings.LineupGenerator.Business.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DraftKings.LineupGenerator.Business
{
    public static class ServiceRegistry
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            // Add Api Services
            services
                .AddHttpClient()
                .AddMemoryCache()
                .AddTransient<IRulesClient, RulesClient>()
                .AddTransient<IContestsClient, ContestsClient>()
                .AddTransient<IDraftablesClient, DraftablesClient>()
                .AddTransient<IDraftKingsClient, DraftKingsClient>();

            // Add Business Services
            services
                .AddTransient<IClassicLineupService, ClassicLineupService>()
                .AddTransient<ILineupGeneratorService, LineupGeneratorService>();

            // Add Lineup Generators
            services
                .AddTransient<ILineupGenerator, DefaultSalaryCapClassicLineupGenerator>();

            return services;
        }
    }
}
