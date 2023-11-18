using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Models.Constants;
using DraftKings.LineupGenerator.Models.Contests;
using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Lineups;
using DraftKings.LineupGenerator.Models.Rules;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.UI.Pages
{
    public partial class ContestPage : ContentPage
    {
        private Task GeneratorTask { get; set; }
        private ILineupGeneratorService LineupGeneratorService { get; set; }
        private CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();

        private readonly RulesModel _rules;
        private readonly ContestModel _contest;
        private readonly DraftablesModel _draftables;

        public ContestPage(
            RulesModel rules,
            ContestModel contest,
            DraftablesModel draftables)
        {

            _rules = rules;
            _contest = contest;
            _draftables = draftables;

            InitializeComponent();

            ContestNameLbl.Text = _contest.ContestDetail.Name;
            ContestDescriptionLbl.Text = _contest.ContestDetail.ContestSummary;
            RulesNameLbl.Text = $"{_contest.ContestDetail.Sport} | {_rules.GameTypeName}";
            RulesDescriptionLbl.Text = _rules.GameTypeDescription;

            if (_rules.GameTypeName != GameTypes.Showdown)
            {
                ExcludeDefenseGrp.IsVisible = false;
                ExcludeKickersGrp.IsVisible = false;
            }
        }

        private void OnGenerateClicked(object sender, EventArgs e)
        {
            CancellationTokenSource = new CancellationTokenSource();

            ToggleActivity(true);

            LineupGeneratorService = Handler.MauiContext.Services.GetRequiredService<ILineupGeneratorService>();

            GeneratorTask = Task.Factory.StartNew(async () =>
            {
                await GenerateAsync();
            }, TaskCreationOptions.LongRunning);

            Task.Factory.StartNew(async () =>
            {
                while (!CancellationTokenSource.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(3), CancellationTokenSource.Token);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        var (generator, iterationCount, validLineupCount) = LineupGeneratorService.GetProgress().FirstOrDefault();

                        if (iterationCount > 0)
                        {
                            UpdateProgress(iterationCount, validLineupCount);
                        }
                    });
                }
            }, TaskCreationOptions.LongRunning);
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            CancellationTokenSource.Cancel();
            
            if (GeneratorTask != null)
            {
                await GeneratorTask;
            }
        }

        private void ToggleActivity(bool isRunning)
        {
            UpdateProgress(0, 0);
            IterationsGrp.IsVisible = ValidLineupsGrp.IsVisible = isRunning;

            ActivityGrp.IsVisible = ActivityInd.IsRunning = isRunning;

            GenerateBtn.IsVisible = !isRunning;
        }

        private async Task GenerateAsync()
        {
            var request = new LineupRequestModel(_contest.Id)
            {
                IncludeQuestionable = IncludeQuestionableCtrl.IsChecked,
                IncludeBaseSalary = IncludeBaseSalaryCtrl.IsChecked,
                ExcludeDefense = ExcludeDefenseCtrl.IsChecked,
                ExcludeKickers = ExcludeKickersCtrl.IsChecked
            };

            await LineupGeneratorService.GetAsync(request, CancellationTokenSource.Token);

            MainThread.BeginInvokeOnMainThread(() => ToggleActivity(false));
        }

        private void UpdateProgress(long iterationCount, long validLineupCount)
        {
            IterationCountLbl.Text = iterationCount.ToString("n0");
            ValidLineupCountLbl.Text = validLineupCount.ToString("n0");
        }
    }
}
