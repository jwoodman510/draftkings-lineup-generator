using DraftKings.LineupGenerator.Business.Constants;
using DraftKings.LineupGenerator.Business.LineupGenerators.SalaryCap.Classic;
using DraftKings.LineupGenerator.Business.LineupLoggers;
using DraftKings.LineupGenerator.Business.Services;
using DraftKings.LineupGenerator.Constants;
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
    public class DefaultSalaryCapClassicLineupGeneratorTests
    {
        private readonly DefaultSalaryCapClassicLineupGenerator _generator;

        public DefaultSalaryCapClassicLineupGeneratorTests()
        {
            _generator = new DefaultSalaryCapClassicLineupGenerator(
                new ClassicLineupService(),
                new ConsoleIncrementalLineupLogger());
        }

        [Theory]
        [InlineData(DraftTypes.SalaryCap, GameTypes.NflClassic, true)]
        [InlineData(DraftTypes.SalaryCap, GameTypes.NflShowdown, false)]
        [InlineData(DraftTypes.SalaryCap, GameTypes.XflClassic, true)]
        [InlineData(DraftTypes.SalaryCap, GameTypes.XflShowdown, false)]
        [InlineData(DraftTypes.SalaryCap, GameTypes.MaddenClassic, true)]
        [InlineData(DraftTypes.SalaryCap, GameTypes.MaddenShowdown, false)]
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
            var rules = await JsonContentProvider.GetSalaryCapXflClassicRulesAsync();
            var draftables = await JsonContentProvider.GetSalaryCapXflClassicDraftablesAsync();

            var result = await _generator.GenerateAsync(new LineupRequestModel(1), new ContestModel(), rules, draftables, default);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task HonorsRosterSlotTemplate()
        {
            var rules = await JsonContentProvider.GetSalaryCapXflClassicRulesAsync();
            var draftables = await JsonContentProvider.GetSalaryCapXflClassicDraftablesAsync();

            var result = await _generator.GenerateAsync(new LineupRequestModel(1), new ContestModel(), rules, draftables, default);

            var expectedPositions = new List<string>
            {
                "QB",
                "RB",
                "WR/TE",
                "WR/TE",
                "FLEX",
                "FLEX",
                "DST"
            };

            result.Lineups.Should().NotBeEmpty();

            result.Lineups.First().Draftables.Select(x => x.RosterPosition)
                .Should().BeEquivalentTo(expectedPositions, opts => opts.WithoutStrictOrdering());
        }
    }
}