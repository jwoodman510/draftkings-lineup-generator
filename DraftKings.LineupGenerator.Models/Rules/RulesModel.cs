using System.Collections.Generic;

namespace DraftKings.LineupGenerator.Models.Rules
{
    public class RulesModel
    {
        public string GameTypeName { get; set; }

        public string DraftType { get; set; }

        public SalaryCapModel SalaryCap { get; set; }

        public List<LineupTemplateItemModel> LineupTemplate { get; set; }
    }
}