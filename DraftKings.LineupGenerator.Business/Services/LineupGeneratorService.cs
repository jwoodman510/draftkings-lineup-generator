using DraftKings.LineupGenerator.Api;
using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Models.Lineups;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Business.Services
{
    public class LineupGeneratorService : ILineupGeneratorService
    {
        private readonly IDraftKingsClient _draftKingsClient;
        private readonly IEnumerable<ILineupGenerator> _lineupGenerators;

        public LineupGeneratorService(
            IDraftKingsClient draftKingsClient,
            IEnumerable<ILineupGenerator> lineupGenerators)
        {
            _draftKingsClient = draftKingsClient;
            _lineupGenerators = lineupGenerators;
        }

        public async Task<List<LineupsModel>> GetAsync(LineupRequestModel request)
        {
            var rules = await _draftKingsClient.Rules.GetAsync(request.ContestId);
            var draftables = await _draftKingsClient.Draftables.GetAsync(request.ContestId);

            if (rules == null || draftables == null)
            {
                return new List<LineupsModel>();
            }

            var generateLineups = _lineupGenerators
                .Where(x => x.CanGenerate(rules))
                .Select(x => x.GenerateAsync(request, rules, draftables));

            var lineups = await Task.WhenAll(generateLineups);

            return lineups.ToList();
        }
    }
}
