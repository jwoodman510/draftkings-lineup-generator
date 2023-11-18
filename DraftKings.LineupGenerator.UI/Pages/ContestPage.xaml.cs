using DraftKings.LineupGenerator.Models.Contests;
using DraftKings.LineupGenerator.Models.Rules;
using Microsoft.Maui.Controls;

namespace DraftKings.LineupGenerator.UI.Pages
{
    public partial class ContestPage : ContentPage
    {
        private readonly RulesModel _rules;
        private readonly ContestModel _contest;

        public ContestPage(
            RulesModel rules,
            ContestModel contest)
        {
            _rules = rules;
            _contest = contest;

            InitializeComponent();

            ContestNameLbl.Text = _contest.ContestDetail.Name;
            ContestDescriptionLbl.Text = _contest.ContestDetail.ContestSummary;
            RulesNameLbl.Text = $"{_contest.ContestDetail.Sport} | {_rules.GameTypeName}";
            RulesDescriptionLbl.Text = _rules.GameTypeDescription;
        }
    }
}
