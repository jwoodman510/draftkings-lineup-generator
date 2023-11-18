using DraftKings.LineupGenerator.Models.Contests;
using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Rules;
using Microsoft.Maui.Controls;

namespace DraftKings.LineupGenerator.UI.Pages
{
    public partial class ContestPage : ContentPage
    {
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
        }
    }
}
