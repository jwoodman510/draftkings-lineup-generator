using DraftKings.LineupGenerator.Api;
using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Models.Lineups;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        public async Task<List<LineupsModel>> GetAsync(LineupRequestModel request, CancellationToken cancellationToken)
        {
            var contest = await _draftKingsClient.Contests.GetAsync(request.ContestId, cancellationToken);

            if (contest == null)
            {
                return new List<LineupsModel>();
            }

            var rules = await _draftKingsClient.Rules.GetAsync(contest.ContestDetail.GameTypeId, cancellationToken);
            var draftables = await _draftKingsClient.Draftables.GetAsync(contest.ContestDetail.DraftGroupId, cancellationToken);

            if (rules == null || draftables == null)
            {
                return new List<LineupsModel>();
            }

            var generators = _lineupGenerators.Where(x => x.CanGenerate(contest, rules)).AsParallel();

            var tasks = generators.Select(x => x.GenerateAsync(request, contest, rules, draftables, cancellationToken));

            var lineups = await Task.WhenAll(tasks);

            return lineups.ToList();
        }
    }
}
