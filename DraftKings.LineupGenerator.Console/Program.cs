using DraftKings.LineupGenerator.Business;
using DraftKings.LineupGenerator.Business.Extensions;
using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Business.Logging;
using DraftKings.LineupGenerator.Models.Lineups;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
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
            Log.Logger = SerilogConfiguration.Build().CreateLogger();

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
            var giveMeCaptainOption = new Option<string>(new string[] { "--give-me-captain", "-gc" }, "(Showdown Only) The names of captain players to include in generated lineups (comma delimited)");

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
            rootCommand.AddOption(giveMeCaptainOption);

            var modelBinder = new LineupRequestModelBinder(
                contestIdOption,
                includeQuestionableOption,
                includeBaseSalaryOption,
                minFppgOption,
                excludeDefenseOption,
                excludeKickersOption,
                outputFormatOption,
                lineupCountOption,
                giveMeOption,
                giveMeCaptainOption);

            rootCommand.SetHandler(async request =>
            {
                var serviceProvider = new ServiceCollection()
                    .RegisterServices()
                    .BuildServiceProvider();

                var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                var lineupGeneratorService = serviceProvider.GetRequiredService<ILineupGeneratorService>();                

                _ = Task.Factory.StartNew(async () =>
                {
                    var key = Console.ReadKey().KeyChar;

                    while (!CancellationTokenSource.IsCancellationRequested)
                    {
                        if (key is 'q')
                        {
                            CancellationTokenSource.Cancel();
                        }
                        else if (key is 'p')
                        {
                            await lineupGeneratorService.LogProgressAsync(request, CancellationTokenSource.Token);
                        }

                        key = Console.ReadKey().KeyChar;
                    }
                });

                var lineups = await lineupGeneratorService.GetAsync(request, CancellationTokenSource.Token);

                if (lineups.Any(x => x.Lineups?.Count > 0))
                {
                    await lineupGeneratorService.LogProgressAsync(request, CancellationTokenSource.Token);
                }
                else
                {
                    logger.LogError("No Lineups Found.");
                }

                Log.CloseAndFlush();

            }, modelBinder);

            await rootCommand.InvokeAsync(args);
        }
    }
}