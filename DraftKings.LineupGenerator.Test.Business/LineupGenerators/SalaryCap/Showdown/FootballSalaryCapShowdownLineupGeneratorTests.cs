using DraftKings.LineupGenerator.Business.LineupGenerators.SalaryCap.Classic;
using DraftKings.LineupGenerator.Business.Logging;
using DraftKings.LineupGenerator.Business.Services;
using DraftKings.LineupGenerator.Models.Constants;
using DraftKings.LineupGenerator.Models.Contests;
using DraftKings.LineupGenerator.Models.Lineups;
using DraftKings.LineupGenerator.Models.Rules;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DraftKings.LineupGenerator.Test.Business
{
    public class FootballSalaryCapShowdownLineupGeneratorTests
    {
        private readonly FootballSalaryCapShowdownLineupGenerator _generator;

        public FootballSalaryCapShowdownLineupGeneratorTests()
        {
            _generator = new FootballSalaryCapShowdownLineupGenerator(
                new ShowdownLineupService(),
                new IncrementalLineupLogger(default, default));
        }

        [Theory]
        [InlineData(DraftTypes.SalaryCap, GameTypes.NflClassic, false)]
        [InlineData(DraftTypes.SalaryCap, GameTypes.NflShowdown, true)]
        [InlineData(DraftTypes.SalaryCap, GameTypes.XflClassic, false)]
        [InlineData(DraftTypes.SalaryCap, GameTypes.XflShowdown, true)]
        [InlineData(DraftTypes.SalaryCap, GameTypes.MaddenClassic, false)]
        [InlineData(DraftTypes.SalaryCap, GameTypes.MaddenShowdown, true)]
        public void CanGenerateGameTypes(string draftType, string gameType, bool expected)
        {
            var contest = new ContestModel
            {
                ContestDetail = new ContestDetailModel
                {
                    Sport = Sports.Nfl
                }
            };

            var rules = new RulesModel
            {
                DraftType = draftType,
                GameTypeName = gameType,
                SalaryCap = new SalaryCapModel
                {
                    IsEnabled = true
                }
            };

            var canGenerate = _generator.CanGenerate(contest, rules);

            Assert.Equal(expected, canGenerate);
        }

        [Fact]
        public async Task GeneratesLineup()
        {
            var rules = await JsonContentProvider.GetSalaryCapMaddenShowdownRulesAsync();
            var draftables = await JsonContentProvider.GetSalaryCapMaddenShowdownDraftablesAsync();

            var result = await _generator.GenerateAsync(new LineupRequestModel(1), new ContestModel(), rules, draftables, default);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task HonorsRosterSlotTemplate()
        {
            var rules = await JsonContentProvider.GetSalaryCapMaddenShowdownRulesAsync();
            var draftables = await JsonContentProvider.GetSalaryCapMaddenShowdownDraftablesAsync();

            var result = await _generator.GenerateAsync(new LineupRequestModel(1) { MinFppg = 5.0m }, new ContestModel(), rules, draftables, default);

            var expectedPositions = new List<string>
            {
                "CPT",
                "FLEX",
                "FLEX",
                "FLEX",
                "FLEX",
                "FLEX"
            };

            result.Should().NotBeEmpty();

            result.First().Lineups.Should().NotBeEmpty();

            result.First().Lineups.First().Draftables.Select(x => x.RosterPosition)
                .Should().BeEquivalentTo(expectedPositions, opts => opts.WithoutStrictOrdering());
        }
    }
}