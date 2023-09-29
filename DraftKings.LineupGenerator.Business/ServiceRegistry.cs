using DraftKings.LineupGenerator.Api;
using DraftKings.LineupGenerator.Api.Draftables;
using DraftKings.LineupGenerator.Api.Rules;
using DraftKings.LineupGenerator.Business.OutputFormatters;
using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Business.LineupGenerators.SalaryCap.Classic;
using DraftKings.LineupGenerator.Business.Services;
using DraftKings.LineupGenerator.Caching;
using DraftKings.LineupGenerator.Caching.Services;
using Microsoft.Extensions.DependencyInjection;
using DraftKings.LineupGenerator.Business.LineupLoggers;

namespace DraftKings.LineupGenerator.Business
{
    public static class ServiceRegistry
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            // Add Caching
            services
                .AddMemoryCache()
                .AddTransient<ICacheService, InMemoryCacheService>()
                .AddTransient<ICacheService, FileBasedCacheService>();

            // Add Api Services
            services
                .AddHttpClient()
                .AddTransient<IRulesClient, RulesClient>()
                .AddTransient<IContestsClient, ContestsClient>()
                .AddTransient<IDraftablesClient, DraftablesClient>()
                .AddTransient<IDraftKingsClient, DraftKingsClient>();

            // Add Business Services
            services
                .AddScoped<IClassicLineupService, ClassicLineupService>()
                .AddScoped<IShowdownLineupService, ShowdownLineupService>()
                .AddScoped<ILineupGeneratorService, LineupGeneratorService>();

            // Add Lineup Generators
            services
                .AddScoped<ILineupGenerator, DefaultSalaryCapClassicLineupGenerator>()
                .AddScoped<ILineupGenerator, DefaultSalaryCapShowdownLineupGenerator>();

            // Add Formatters
            services
                .AddScoped<IOutputFormatter, TextOutputFormatter>()
                .AddScoped<IOutputFormatter, JsonOutputFormatter>();

            // Add Lineup Loggers
            services
                .AddScoped<IIncrementalLineupLogger, ConsoleIncrementalLineupLogger>();

            return services;
        }
    }
}
