using DraftKings.LineupGenerator.Api.Draftables;
using DraftKings.LineupGenerator.Models.Contests;
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

            var contestClient = Handler.MauiContext.Services.GetService<IContestsClient>();

            var contest = await contestClient.GetAsync(contestId, default);

            if (contest == null)
            {
                NotFoundLbl.IsVisible = true;
            }
            else
            {
                // TODO: Route to generator page with ID
            }
        }
    }
}