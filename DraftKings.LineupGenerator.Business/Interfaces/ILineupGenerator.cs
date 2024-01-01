using DraftKings.LineupGenerator.Models.Contests;
using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Lineups;
using DraftKings.LineupGenerator.Models.Rules;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Business.Interfaces
{
    public interface ILineupGenerator
    {
        bool CanGenerate(ContestModel contest, RulesModel rules);

        IEnumerable<LineupsModel> GetCurrentLineups(LineupRequestModel request);

        Task<IEnumerable<LineupsModel>> GenerateAsync(LineupRequestModel request, ContestModel contest, RulesModel rules, DraftablesModel draftables, CancellationToken cancellationToken);

        IEnumerable<LineupsModel> GetCurrentLineups();

        (long iterationCount, long validLineupCount) GetProgress();
    }
}
