using Blazored.LocalStorage;
using DraftKings.LineupGenerator.Models.Contests;

namespace DraftKings.LineupGenerator.Razor.Services
{
    public class RecentContestService : IRecentContestService
    {
        private readonly ILocalStorageService _localStorageService;

        public RecentContestService(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public async Task AddRecentContestAsync(ContestSearchModel contestSearchModel)
        {
            var contests = await _localStorageService.GetItemAsync<Dictionary<int, ContestSearchModel>>(Constants.LocalStorage.RecentContests) ?? [];

            contests[contestSearchModel.Id] = contestSearchModel;

            await _localStorageService.SetItemAsync(Constants.LocalStorage.RecentContests, contests);
        }

        public async Task<ContestSearchModel?> GetRecentContestAsync(int id)
        {
            var contests = await GetRecentContestsAsync();

            return contests.FirstOrDefault(x => x.Id == id);
        }

        public async Task<IEnumerable<ContestSearchModel>> GetRecentContestsAsync()
        {
            var currentContests = await _localStorageService.GetItemAsync<Dictionary<int, ContestSearchModel>>(Constants.LocalStorage.RecentContests);

            return currentContests?.Values?.ToList() ?? [];
        }

        public async Task RemoveRecentContestAsync(int id)
        {
            var contests = await _localStorageService.GetItemAsync<Dictionary<int, ContestSearchModel>>(Constants.LocalStorage.RecentContests);

            if (contests == null)
            {
                return;
            }

            contests.Remove(id);

            await _localStorageService.SetItemAsync(Constants.LocalStorage.RecentContests, contests);
        }

        public async Task RemoveAllAsync()
        {
            await _localStorageService.RemoveItemAsync(Constants.LocalStorage.RecentContests);
        }
    }
}
