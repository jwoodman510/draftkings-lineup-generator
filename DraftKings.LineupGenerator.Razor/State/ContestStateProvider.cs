using DraftKings.LineupGenerator.Api.Draftables;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace DraftKings.LineupGenerator.Razor.State
{
    public class ContestStateProvider
    {
        private readonly IContestsClient _contestsClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<int, ContestState> _stateDictionary = new();

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
    }
}
