using Blazored.LocalStorage;
using DraftKings.LineupGenerator.Razor.Contests;

namespace DraftKings.LineupGenerator.Razor.Services
{
    public class ContestHistoryService : IContestHistoryService
    {
        private readonly ILocalStorageService _localStorageService;

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public ContestHistoryService(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public async Task<ContestHistoryModel?> GetAsync(Guid id)
        {
            var history = await _localStorageService.GetItemAsync<Dictionary<Guid, ContestHistoryModel>>(Constants.LocalStorage.ContestHistory);

            if (history == null)
            {
                return default;
            }

            return history.TryGetValue(id, out var contestHistoryModel) ? contestHistoryModel : default;
        }

        public async Task CreateAsync(ContestHistoryModel contestHistoryModel)
        {
            await _semaphore.WaitAsync();

            try
            {
                var history = await _localStorageService.GetItemAsync<Dictionary<Guid, ContestHistoryModel>>(Constants.LocalStorage.ContestHistory) ?? [];

                history[contestHistoryModel.Id] = contestHistoryModel;

                await _localStorageService.SetItemAsync(Constants.LocalStorage.ContestHistory, history);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task DeleteAsync(ContestHistoryModel contestHistoryModel)
        {
            await _semaphore.WaitAsync();

            try
            {
                var history = await _localStorageService.GetItemAsync<Dictionary<Guid, ContestHistoryModel>>(Constants.LocalStorage.ContestHistory) ?? [];

                history.Remove(contestHistoryModel.Id);

                await _localStorageService.SetItemAsync(Constants.LocalStorage.ContestHistory, history);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<IEnumerable<ContestHistoryModel>> GetAsync()
        {
            var history = await _localStorageService.GetItemAsync<Dictionary<Guid, ContestHistoryModel>>(Constants.LocalStorage.ContestHistory) ?? [];

            return history.Values;
        }

        public async Task UpdateAsync(ContestHistoryModel contestHistoryModel)
        {
            await _semaphore.WaitAsync();

            try
            {
                var history = await _localStorageService.GetItemAsync<Dictionary<Guid, ContestHistoryModel>>(Constants.LocalStorage.ContestHistory) ?? [];

                history[contestHistoryModel.Id] = contestHistoryModel;

                await _localStorageService.SetItemAsync(Constants.LocalStorage.ContestHistory, history);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
