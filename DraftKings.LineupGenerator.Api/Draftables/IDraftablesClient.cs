using DraftKings.LineupGenerator.Models.Draftables;
using System.Threading.Tasks;

namespace DraftKings.LineupGenerator.Api.Draftables
{
    public interface IDraftablesClient
    {
        Task<DraftablesModel> GetAsync(int contestId);
    }
}
