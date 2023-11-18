using DraftKings.LineupGenerator.Api;
using DraftKings.LineupGenerator.UI.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using System;

namespace DraftKings.LineupGenerator.UI
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

#if DEBUG
            if (ContestEntry != null)
            {
                ContestEntry.Text = "154230362";
            }
#endif
        }

        private async void OnFindContestClicked(object sender, EventArgs e)
        {
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
                var text = JsonConvert.SerializeObject(new
                {

                });

                await DisplayAlert("Alert", text, "OK");

                return;
            }

            var rules = await draftKingsClient.Rules.GetAsync(contest.ContestDetail.GameTypeId, default);
            var draftables = await draftKingsClient.Draftables.GetAsync(contest.ContestDetail.DraftGroupId, default);

            await Navigation.PushAsync(new ContestPage(rules, contest, draftables), true);
        }
    }
}