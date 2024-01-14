using DraftKings.LineupGenerator.Api.Draftables;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace DraftKings.LineupGenerator.Razor.Contests
{
    public class ContestStateProvider
    {
        private readonly IContestsClient _contestsClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<int, ContestState> _stateDictionary = new();
        private readonly ConcurrentDictionary<Guid, ContestState> _historyDictionary = new();

        public ContestStateProvider(
            IContestsClient contestsClient,
            IServiceProvider serviceProvider)
        {
            _contestsClient = contestsClient;
            _serviceProvider = serviceProvider;
        }

        public async Task<ContestState> GetOrCreateAsync(int Id)
        {
            if (_stateDictionary.TryGetValue(Id, out var state))
            {
                return state;
            }

            var contestModel = await _contestsClient.GetAsync(Id, default).ConfigureAwait(true);

            state = new ContestState(contestModel, _serviceProvider.CreateScope());

            _stateDictionary[Id] = state;

            return state;
        }

        public Task<ContestState> GetOrCreateAsync(ContestHistoryModel contestHistoryModel)
        {
            if (_historyDictionary.TryGetValue(contestHistoryModel.Id, out var state))
            {
                return Task.FromResult(state);
            }

            state = new ContestState(contestHistoryModel, _serviceProvider.CreateScope());

            _historyDictionary[contestHistoryModel.Id] = state;

            return Task.FromResult(state);
        }

        public void Reset()
        {
            _historyDictionary.Clear();
            
            foreach (var state in _stateDictionary.Values)
            {
                state.Dispose();
            }

            _stateDictionary.Clear();
        }
    }
}
