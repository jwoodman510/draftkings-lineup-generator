using Blazored.LocalStorage;
using DraftKings.LineupGenerator.Models.Contests;

namespace DraftKings.LineupGenerator.Razor.Services
{
    public class ContestService : IContestService
    {
        private readonly ILocalStorageService _localStorageService;

        public ContestService(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public async Task AddRecentContestAsync(ContestSearchModel contestSearchModel)
        {
            var contests = await _localStorageService.GetItemAsync<Dictionary<int, ContestSearchModel>>(Constants.LocalStorage.Contests) ?? [];

            contests[contestSearchModel.Id] = contestSearchModel;

            await _localStorageService.SetItemAsync(Constants.LocalStorage.Contests, contests);
        }

        public async Task<ContestSearchModel?> GetRecentContestAsync(int id)
        {
            var contests = await GetRecentContestsAsync();

            return contests.FirstOrDefault(x => x.Id == id);
        }

        public async Task<IEnumerable<ContestSearchModel>> GetRecentContestsAsync()
        {
            var currentContests = await _localStorageService.GetItemAsync<Dictionary<int, ContestSearchModel>>(Constants.LocalStorage.Contests);

            return currentContests?.Values?.ToList() ?? [];
        }

        public async Task RemoveRecentContestAsync(int id)
        {
            var contests = await _localStorageService.GetItemAsync<Dictionary<int, ContestSearchModel>>(Constants.LocalStorage.Contests);

            if (contests == null)
            {
                return;
            }

            contests.Remove(id);

            await _localStorageService.SetItemAsync(Constants.LocalStorage.Contests, contests);
        }
    }
}
