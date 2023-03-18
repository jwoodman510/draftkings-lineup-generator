using DraftKings.LineupGenerator.Business;
using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Console;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.CommandLine;
using System.Threading.Tasks;

namespace draftkings_lineup_generator
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var rootCommand = new RootCommand();

            var contestIdOption = new Option<int>("--contestId", "[REQUIRED] The DraftKings contest identifier.");
            var includeQuestionableOption = new Option<bool>("--include-questionable", "Includes draftables with a questionable status.");

            rootCommand.AddOption(contestIdOption);
            rootCommand.AddOption(includeQuestionableOption);

            var modelBinder = new LineupRequestModelBinder(contestIdOption, includeQuestionableOption);

            rootCommand.SetHandler(async request =>
            {
                Console.WriteLine("Generating Lineups for Configuration:");
                Console.WriteLine(JsonConvert.SerializeObject(request, Formatting.Indented));

                var lineups = await new ServiceCollection()
                    .RegisterServices()
                    .BuildServiceProvider()
                    .GetRequiredService<ILineupsService>()
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