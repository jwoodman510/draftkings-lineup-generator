using DraftKings.LineupGenerator.Api.Draftables;
using DraftKings.LineupGenerator.Api.Rules;
using DraftKings.LineupGenerator.UI.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using System;

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

            var contest = await Handler.MauiContext.Services
                .GetService<IContestsClient>()
                .GetAsync(contestId, default);

            if (contest == null)
            {
                NotFoundLbl.IsVisible = true;

                return;
            }

            var rules = await Handler.MauiContext.Services
                .GetService<IRulesClient>()
                .GetAsync(contest.ContestDetail.GameTypeId, default);

            await Navigation.PushAsync(new ContestPage(rules, contest), true);
        }
    }
}