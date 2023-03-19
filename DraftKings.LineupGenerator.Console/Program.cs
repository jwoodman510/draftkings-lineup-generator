using DraftKings.LineupGenerator.Business;
using DraftKings.LineupGenerator.Business.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.CommandLine;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var rootCommand = new RootCommand();

            var contestIdOption = new Option<int>("--contestId", "[REQUIRED] The DraftKings contest identifier.");
            var includeQuestionableOption = new Option<bool>("--include-questionable", "[default=false] Includes draftables with a questionable status.");
            var includeBaseSalaryOption = new Option<bool>("--include-base-salary", "[default=false] Includes draftables with the base salary (lowest possible).");
            var minFppgOption = new Option<decimal>("--min-fppg", "[default=5.0] Minimum fantasy points per game (per player).");

            rootCommand.AddOption(contestIdOption);
            rootCommand.AddOption(includeQuestionableOption);
            rootCommand.AddOption(includeBaseSalaryOption);
            rootCommand.AddOption(minFppgOption);

            var modelBinder = new LineupRequestModelBinder(
                contestIdOption,
                includeQuestionableOption,
                includeBaseSalaryOption,
                minFppgOption);

            rootCommand.SetHandler(async request =>
            {
                Console.WriteLine("Generating Lineups for Configuration:");
                Console.WriteLine(JsonConvert.SerializeObject(request, Formatting.Indented));

                var lineups = await new ServiceCollection()
                    .RegisterServices()
                    .BuildServiceProvider()
                    .GetRequiredService<ILineupGeneratorService>()
                    .GetAsync(request);

                var json = JsonConvert.SerializeObject(lineups, Formatting.Indented);

                Console.WriteLine("Lineups Generated:");

                Console.WriteLine(json);

            }, modelBinder);

            await rootCommand.InvokeAsync(args);

            Console.ReadLine();
        }
    }
}