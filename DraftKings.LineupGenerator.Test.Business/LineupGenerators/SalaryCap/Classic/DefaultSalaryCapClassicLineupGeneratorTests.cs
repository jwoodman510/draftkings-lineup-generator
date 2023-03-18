using DraftKings.LineupGenerator.Business.LineupGenerators.SalaryCap.Classic;
using DraftKings.LineupGenerator.Constants;
using DraftKings.LineupGenerator.Models.Lineups;
using DraftKings.LineupGenerator.Models.Rules;
using System.Threading.Tasks;
using Xunit;

namespace DraftKings.LineupGenerator.Test.Business
{
    public class DefaultSalaryCapClassicLineupGeneratorTests
    {
        private readonly DefaultSalaryCapClassicLineupGenerator _generator;

        public DefaultSalaryCapClassicLineupGeneratorTests()
        {
            _generator = new DefaultSalaryCapClassicLineupGenerator();
        }

        [Theory]
        [InlineData(DraftTypes.SalaryCap, GameTypes.NflClassic, true)]
        [InlineData(DraftTypes.SalaryCap, GameTypes.NflShowDown, false)]
        [InlineData(DraftTypes.SalaryCap, GameTypes.XflClassic, true)]
        [InlineData(DraftTypes.SalaryCap, GameTypes.XflShowDown, false)]
        public void CanGenerateContest(string draftType, string gameType, bool expected)
        {
            var rules = new RulesModel
            {
                DraftType = draftType,
                GameTypeName = gameType,
            };

            var canGenerate = _generator.CanGenerate(rules);

            Assert.Equal(expected, canGenerate);
        }

        [Fact]
        public async Task GeneratesLineup()
        {
            var rules = await JsonContentProvider.GetSalaryCapXflClassicRulesAsync();
            var draftables = await JsonContentProvider.GetSalaryCapXflClassicDraftablesAsync();

            var lineup = await _generator.GenerateAsync(new LineupRequestModel(1), rules, draftables);

            Assert.NotNull(lineup);
        }
    }
}