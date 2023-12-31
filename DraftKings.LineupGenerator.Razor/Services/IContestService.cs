using DraftKings.LineupGenerator.Models.Contests;

namespace DraftKings.LineupGenerator.Razor.Services
{
    public interface IContestService
    {
        Task AddRecentContestAsync(ContestSearchModel contestSearchModel);

        Task<ContestSearchModel?> GetRecentContestAsync(int id);

        Task<IEnumerable<ContestSearchModel>> GetRecentContestsAsync();

        Task RemoveRecentContestAsync(int id);
    }
}
