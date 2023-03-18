using DraftKings.LineupGenerator.Models.Draftables;
using DraftKings.LineupGenerator.Models.Lineups;
using DraftKings.LineupGenerator.Models.Rules;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Business.Interfaces
{
    public interface ILineupGenerator
    {
        bool CanGenerate(RulesModel rules);

        Task<LineupsModel> GenerateAsync(LineupRequestModel request, RulesModel rules, DraftablesModel draftables);
    }
}
