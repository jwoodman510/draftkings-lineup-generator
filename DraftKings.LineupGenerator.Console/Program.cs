using DraftKings.LineupGenerator.Business;
using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Models.Lineups;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var rootCommand = new RootCommand();

            var contestIdOption = new Option<int>("--contestId", "[REQUIRED] The DraftKings contest identifier.")
            {
                IsRequired = true
            };

            var includeQuestionableOption = new Option<bool>("--include-questionable", "[default=false] Includes draftables with a questionable status.");
            var includeBaseSalaryOption = new Option<bool>("--include-base-salary", "[default=false] Includes draftables with the base salary (lowest possible).");
            var minFppgOption = new Option<decimal>("--min-fppg", "(Classic Only) [default=10.0] Minimum fantasy points per game (per player - excluding defense & kickers).");
            var excludeDefenseOption = new Option<bool>("--exclude-defense", "(Showdown Only) [default: false] Excludes DST positions from lineups.");
            var excludeKickersOption = new Option<bool>("--exclude-kickers", "(Showdown Only) [default=false] Excludes Kicker positions from lineups.");
            var outputFormatOption = new Option<string>("--output-format", "[default=text] The console output format. One of (json | text)");

            minFppgOption.SetDefaultValue(new LineupRequestModel(default).MinFppg);

            rootCommand.AddOption(contestIdOption);
            rootCommand.AddOption(includeQuestionableOption);
            rootCommand.AddOption(includeBaseSalaryOption);
            rootCommand.AddOption(minFppgOption);
            rootCommand.AddOption(excludeDefenseOption);
            rootCommand.AddOption(excludeKickersOption);
            rootCommand.AddOption(outputFormatOption);

            var modelBinder = new LineupRequestModelBinder(
                contestIdOption,
                includeQuestionableOption,
                includeBaseSalaryOption,
                minFppgOption,
                excludeDefenseOption,
                excludeKickersOption,
                outputFormatOption);

            rootCommand.SetHandler(async request =>
            {
                Console.WriteLine("Generating Lineups for Configuration:");
                Console.WriteLine(JsonConvert.SerializeObject(request, Formatting.Indented));

                var serviceProvider = new ServiceCollection()
                    .RegisterServices()
                    .BuildServiceProvider();

                var lineups = await serviceProvider
                    .GetRequiredService<ILineupGeneratorService>()
                    .GetAsync(request);

                Console.WriteLine("Lineups Generated:");

                var formatters = serviceProvider.GetServices<IOutputFormatter>();

                var formatter = formatters.FirstOrDefault(x => x.Type.Equals(request.OutputFormat, StringComparison.OrdinalIgnoreCase)) ?? formatters.First();

                var output = await formatter.FormatAsync(lineups);

                Console.WriteLine(output);

            }, modelBinder);

            await rootCommand.InvokeAsync(args);

            Console.ReadLine();
        }
    }
}