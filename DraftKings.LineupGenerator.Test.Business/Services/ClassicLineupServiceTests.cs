using DraftKings.LineupGenerator.Business.Services;
using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Lineups;
using DraftKings.LineupGenerator.Models.Rules;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DraftKings.LineupGenerator.Test.Business.Services
{
    public class ClassicLineupServiceTests
    {
        private readonly ClassicLineupService _service = new ClassicLineupService();

        [Fact]
        public void GetsAllPermutations()
        {
            var lineups = _service.GetPotentialLineups(
                new LineupRequestModel(1),
                new RulesModel
                {
                    LineupTemplate = new List<LineupTemplateItemModel>
                    {
                        new LineupTemplateItemModel { RosterSlot = new RosterSlotModel { Id = 1 } },
                        new LineupTemplateItemModel { RosterSlot = new RosterSlotModel { Id = 2 } }
                    }
                },
                new DraftablesModel(),
                new List<DraftableModel>
                {
                    new DraftableModel { RosterSlotId = 1, DraftableId = 1, PlayerId = 1 },
                    new DraftableModel { RosterSlotId = 1, DraftableId = 2, PlayerId = 2 },
                    new DraftableModel { RosterSlotId = 2, DraftableId = 3, PlayerId = 3 }
                });

            lineups.Should().HaveCount(2);

            lineups.Should().ContainEquivalentOf(new List<DraftableModel>
            {
                new DraftableModel { RosterSlotId = 1, DraftableId= 1, PlayerId = 1 },
                new DraftableModel { RosterSlotId = 2, DraftableId= 3, PlayerId = 3 }
            });

            lineups.Should().ContainEquivalentOf(new List<DraftableModel>
            {
                new DraftableModel { RosterSlotId = 1, DraftableId= 2, PlayerId = 2 },
                new DraftableModel { RosterSlotId = 2, DraftableId= 3, PlayerId = 3 }
            });
        }

        [Fact]
        public void SkipsPermutationsWithDuplicatePlayerIds()
        {
            var lineups = _service.GetPotentialLineups(
                new LineupRequestModel(1),
                new RulesModel
                {
                    LineupTemplate = new List<LineupTemplateItemModel>
                    {
                        new LineupTemplateItemModel { RosterSlot = new RosterSlotModel { Id = 1 } },
                        new LineupTemplateItemModel { RosterSlot = new RosterSlotModel { Id = 2 } }
                    }
                },
                new DraftablesModel(),
                new List<DraftableModel>
                {
                    new DraftableModel { RosterSlotId = 1, DraftableId = 1, PlayerId = 1 },
                    new DraftableModel { RosterSlotId = 1, DraftableId = 2, PlayerId = 2 },
                    new DraftableModel { RosterSlotId = 2, DraftableId = 2, PlayerId = 2 }
                });

            lineups.Should().HaveCount(1);

            lineups.Should().ContainEquivalentOf(new List<DraftableModel>
            {
                new DraftableModel { RosterSlotId = 1, DraftableId= 1, PlayerId = 1 },
                new DraftableModel { RosterSlotId = 2, DraftableId= 2, PlayerId = 2 }
            });
        }

        [Fact]
        public void FiltersPermutationsByPlayerRequest()
        {
            var lineups = _service.GetPotentialLineups(
                new LineupRequestModel(1)
                {
                    PlayerRequests = new PlayerRequestsModel
                    {
                        PlayerNameRequests = new HashSet<string>
                        {
                            "Joe"
                        }
                    }
                },
                new RulesModel
                {
                    LineupTemplate = new List<LineupTemplateItemModel>
                    {
                        new LineupTemplateItemModel { RosterSlot = new RosterSlotModel { Id = 1 } },
                        new LineupTemplateItemModel { RosterSlot = new RosterSlotModel { Id = 2 } }
                    }
                },
                new DraftablesModel(),
                new List<DraftableModel>
                {
                    new DraftableModel { RosterSlotId = 1, DraftableId = 1, PlayerId = 1, FirstName = "Joe", LastName = "Schmoe", DisplayName = "J. Schmoe" },
                    new DraftableModel { RosterSlotId = 1, DraftableId = 2, PlayerId = 2, FirstName = "Bob", LastName = "Doe", DisplayName = "B. Doe" },
                    new DraftableModel { RosterSlotId = 2, DraftableId = 3, PlayerId = 3, FirstName = "Allen", LastName = "Boe", DisplayName = "A. Boe" }
                });

            lineups.Should().HaveCount(1);

            lineups.Should().ContainEquivalentOf(new List<DraftableModel>
            {
                new DraftableModel { RosterSlotId = 1, DraftableId= 1, PlayerId = 1, FirstName = "Joe", LastName = "Schmoe", DisplayName = "J. Schmoe" },
                new DraftableModel { RosterSlotId = 2, DraftableId= 3, PlayerId = 3, FirstName = "Allen", LastName = "Boe", DisplayName = "A. Boe" }
            });
        }
    }
}
