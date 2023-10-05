using DraftKings.LineupGenerator.Business;
using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Models.Lineups;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.CommandLine;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator
{
    internal class Program
    {
        static readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        static async Task Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                Description = ".NET tool used to generate DraftKings lineups."
            };

            var contestIdOption = new Option<int>(new[] { "--contestId", "-c" }, "[REQUIRED] The DraftKings contest identifier.")
            {
                IsRequired = true
            };

            var includeQuestionableOption = new Option<bool>(new string[] { "--include-questionable", "-iq" }, "[default=false] Includes draftables with a questionable status.");
            var includeBaseSalaryOption = new Option<bool>(new string[] { "--include-base-salary", "-ibs" }, "[default=false] Includes draftables with the base salary (lowest possible).");
            var minFppgOption = new Option<decimal>(new string[] { "--min-fppg", "-mp" }, "(Classic Only) [default=10.0] Minimum fantasy points per game (per player - excluding defense & kickers).");
            var excludeDefenseOption = new Option<bool>(new string[] { "--exclude-defense", "-ed" }, "(Showdown Only) [default: false] Excludes DST positions from lineups.");
            var excludeKickersOption = new Option<bool>(new string[] { "--exclude-kickers", "-ek" }, "(Showdown Only) [default=false] Excludes Kicker positions from lineups.");
            var outputFormatOption = new Option<string>(new string[] { "--output-format", "-f" }, "[default=text] The console output format. One of (json | text)");
            var lineupCountOption = new Option<int>(new string[] { "--lineup-count", "-lc" }, "[default=5] The number of lineups to output for each generator (max is 100).");
            var giveMeOption = new Option<string>(new string[] { "--give-me", "-g" }, "The names of players to include in generated lineups (comma delimited)");

            minFppgOption.SetDefaultValue(new LineupRequestModel(default).MinFppg);
            outputFormatOption.SetDefaultValue(new LineupRequestModel(default).OutputFormat);
            lineupCountOption.SetDefaultValue(new LineupRequestModel(default).LineupCount);
            lineupCountOption.AddValidator(result =>
            {
                if (result.GetValueForOption(lineupCountOption) < 1)
                {
                    result.ErrorMessage = "Must be greater than zero.";
                }

                if (result.GetValueForOption(lineupCountOption) > 100)
                {
                    result.ErrorMessage = "Must be less than 100.";
                }
            });

            rootCommand.AddOption(contestIdOption);
            rootCommand.AddOption(includeQuestionableOption);
            rootCommand.AddOption(includeBaseSalaryOption);
            rootCommand.AddOption(minFppgOption);
            rootCommand.AddOption(excludeDefenseOption);
            rootCommand.AddOption(excludeKickersOption);
            rootCommand.AddOption(outputFormatOption);
            rootCommand.AddOption(lineupCountOption);
            rootCommand.AddOption(giveMeOption);

            var modelBinder = new LineupRequestModelBinder(
                contestIdOption,
                includeQuestionableOption,
                includeBaseSalaryOption,
                minFppgOption,
                excludeDefenseOption,
                excludeKickersOption,
                outputFormatOption,
                lineupCountOption,
                giveMeOption);

            rootCommand.SetHandler(async request =>
            {
                var serviceProvider = new ServiceCollection()
                    .RegisterServices()
                    .BuildServiceProvider();

                var formatters = serviceProvider.GetServices<IOutputFormatter>();
                var formatter = formatters.FirstOrDefault(x => x.Type.Equals(request.OutputFormat, StringComparison.OrdinalIgnoreCase)) ?? formatters.First();

                WriteLine("Generating Lineups for Configuration:", ConsoleColor.Green);

                WriteLine(await formatter.FormatAsync(request), ConsoleColor.Cyan);

                _ = Task.Factory.StartNew(async () =>
                {
                    var key = Console.ReadKey().KeyChar;

                    while (!CancellationTokenSource.IsCancellationRequested)
                    {
                        if (key is 'q')
                        {
                            Console.WriteLine();
                            CancellationTokenSource.Cancel();
                        }
                        else if (key is 'p')
                        {
                            Console.WriteLine();

                            var lineupsModels = serviceProvider.GetServices<ILineupGenerator>()
                                .Select(x => x.GetCurrentLineups())
                                .Where(x => x?.Lineups?.Count > 0)
                                .ToList();

                            if (lineupsModels.Count > 0)
                            {
                                foreach (var lineupsModel in lineupsModels)
                                {
                                    WriteLine($"Generator: {lineupsModel.Description}", ConsoleColor.Green);
                                    WriteLine(await formatter.FormatLineupsAsync(lineupsModel.Lineups), ConsoleColor.Cyan);
                                }
                            }
                            else
                            {
                                WriteLine("No Lineups Found.", ConsoleColor.Yellow);
                            }
                        }

                        key = Console.ReadKey().KeyChar;
                    }
                });

                var lineups = await serviceProvider.GetRequiredService<ILineupGeneratorService>()
                    .GetAsync(request, CancellationTokenSource.Token);

                if (lineups.Any(x => x.Lineups?.Count > 0))
                {
                    WriteLine("Lineups Generated:", ConsoleColor.Green);

                    foreach (var lineupsModel in lineups)
                    {
                        WriteLine($"Generator: {lineupsModel.Description}", ConsoleColor.Green);
                        WriteLine(await formatter.FormatLineupsAsync(lineupsModel.Lineups), ConsoleColor.Cyan);
                    }
                }
                else
                {
                    WriteLine("No Lineups Found.", ConsoleColor.Red);
                }


            }, modelBinder);

            await rootCommand.InvokeAsync(args);
        }

        private static void WriteLine(string message, ConsoleColor foregroundColor)
        {
            var defaultColor = Console.ForegroundColor;

            Console.ForegroundColor = foregroundColor;

            Console.WriteLine(message);

            Console.ForegroundColor = defaultColor;
        }
    }
}