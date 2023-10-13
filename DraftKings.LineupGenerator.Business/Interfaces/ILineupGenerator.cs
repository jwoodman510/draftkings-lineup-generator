using DraftKings.LineupGenerator.Models.Contests;
using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Lineups;
using DraftKings.LineupGenerator.Models.Rules;
using System.Threading;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Business.Interfaces
{
    public interface ILineupGenerator
    {
        bool CanGenerate(ContestModel contest, RulesModel rules);

        LineupsModel GetCurrentLineups();

        Task<LineupsModel> GenerateAsync(LineupRequestModel request, ContestModel contest, RulesModel rules, DraftablesModel draftables, CancellationToken cancellationToken);
    }
}
