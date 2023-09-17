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
            var lineupCountOption = new Option<int>("--lineup-count", "[default=5] The number of lineups to output for each generator (max is 100).");

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

            var modelBinder = new LineupRequestModelBinder(
                contestIdOption,
                includeQuestionableOption,
                includeBaseSalaryOption,
                minFppgOption,
                excludeDefenseOption,
                excludeKickersOption,
                outputFormatOption,
                lineupCountOption);

            rootCommand.SetHandler(async request =>
            {
                var serviceProvider = new ServiceCollection()
                    .RegisterServices()
                    .BuildServiceProvider();

                var formatters = serviceProvider.GetServices<IOutputFormatter>();
                var formatter = formatters.FirstOrDefault(x => x.Type.Equals(request.OutputFormat, StringComparison.OrdinalIgnoreCase)) ?? formatters.First();

                WriteLine("Generating Lineups for Configuration:", ConsoleColor.Green);

                WriteLine(await formatter.FormatAsync(request), ConsoleColor.Cyan);

                var lineups = await serviceProvider
                    .GetRequiredService<ILineupGeneratorService>()
                    .GetAsync(request, CancellationTokenSource.Token);

                WriteLine("Lineups Generated:", ConsoleColor.Green);

                Console.WriteLine(await formatter.FormatLineupAsync(lineups));

            }, modelBinder);

            _ = Task.Factory.StartNew(() =>
            {
                if (Console.ReadKey().KeyChar is 'q')
                {
                    CancellationTokenSource.Cancel();
                }
            });

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