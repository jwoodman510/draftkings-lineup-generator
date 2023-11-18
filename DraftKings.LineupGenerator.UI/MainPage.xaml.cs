using DraftKings.LineupGenerator.Api;
using DraftKings.LineupGenerator.Api.Draftables;
using DraftKings.LineupGenerator.Api.Rules;
using DraftKings.LineupGenerator.UI.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using System;
using System.Threading;

namespace DraftKings.LineupGenerator.UI
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnFindContestClicked(object sender, EventArgs e)
        {
            NotFoundLbl.IsVisible = false;

            if (string.IsNullOrEmpty(ContestEntry?.Text))
            {
                return;
            }

            if (!int.TryParse(ContestEntry.Text, out var contestId))
            {
                return;
            }

            var draftKingsClient = Handler.MauiContext.Services.GetRequiredService<IDraftKingsClient>();

            var contest = await draftKingsClient.Contests.GetAsync(contestId, default);

            if (contest == null)
            {
                NotFoundLbl.IsVisible = true;

                return;
            }

            var rules = await draftKingsClient.Rules.GetAsync(contest.ContestDetail.GameTypeId, default);
            var draftables = await draftKingsClient.Draftables.GetAsync(contest.ContestDetail.DraftGroupId, default);

            await Navigation.PushAsync(new ContestPage(rules, contest, draftables), true);
        }
    }
}