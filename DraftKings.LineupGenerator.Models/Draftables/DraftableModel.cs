using System.Collections.Generic;

namespace DraftKings.LineupGenerator.Models.Draftables
{
    public class DraftableModel
    {
        public int DraftableId { get; set; }

        public int RosterSlotId { get; set; }

        public int Salary { get; set; }

        public bool IsDisabled { get; set; }

        public string Position { get; set; }

        public string Status { get; set; }

        public int TeamId { get; set; }

        public int PlayerId { get; set; }

        public string DisplayName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<DraftStatAttributeModel> DraftStatAttributes { get; set; }
    }
}
