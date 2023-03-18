namespace DraftKings.LineupGenerator.Models.Rules
{
    public class LineupTemplateItemModel
    {
        public int Order { get; set; }

        public int ScoringOrder { get; set; }

        public RosterSlotModel RosterSlot { get; set; }
    }
}
