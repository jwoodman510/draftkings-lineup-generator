using DraftKings.LineupGenerator.Razor.Contests;

namespace DraftKings.LineupGenerator.Razor.Services
{
    public interface IContestHistoryService
    {
        Task<ContestHistoryModel?> GetAsync(Guid id);

        Task<IEnumerable<ContestHistoryModel>> GetAsync();

        Task UpdateAsync(ContestHistoryModel contestHistoryModel);

        Task CreateAsync(ContestHistoryModel contestHistoryModel);

        Task DeleteAsync(ContestHistoryModel contestHistoryModel);
    }
}
