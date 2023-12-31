using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Models.Contests;
using DraftKings.LineupGenerator.Models.Lineups;
using Microsoft.Extensions.DependencyInjection;

namespace DraftKings.LineupGenerator.Razor.State
{
    public class ContestState : IDisposable
    {
        public bool IsRunning => _generatorTask != null && !_cancellationTokenSource.IsCancellationRequested;

        public readonly ContestModel ContestModel;

        public readonly LineupRequestModel RequestModel;

        public List<LineupsModel> Results { get; private set; } = [];

        public List<(string generator, long iterationCount, long validLineupCount)> Progress { get; private set; } = [];

        private Task? _generatorTask;
        private Task? _progressTask;
        private CancellationTokenSource _cancellationTokenSource = new();

        private readonly IServiceScope _serviceScope;
        private readonly ILineupGeneratorService _lineupGeneratorService;

        public ContestState(ContestModel contestModel, IServiceScope serviceScope)
        {
            _serviceScope = serviceScope;
            _lineupGeneratorService = serviceScope.ServiceProvider.GetRequiredService<ILineupGeneratorService>();

            ContestModel = contestModel;
            RequestModel = new LineupRequestModel(contestModel.Id);
        }

        public void Generate()
        {
            Results.Clear();
            Progress.Clear();
            _progressTask = null;
            _generatorTask = null;
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            _generatorTask = Task.Factory.StartNew(async () =>
            {
                Results = await _lineupGeneratorService.GetAsync(RequestModel, _cancellationTokenSource.Token);
            }, TaskCreationOptions.LongRunning);

            _progressTask = Task.Factory.StartNew(async () =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    Progress = _lineupGeneratorService.GetProgress().ToList();

                    await Task.Delay(TimeSpan.FromSeconds(10), _cancellationTokenSource.Token);
                }
            });
        }

        public void Cancel()
        {
            Results.Clear();
            Progress.Clear();
            _cancellationTokenSource.Cancel();
            _progressTask = null;
            _generatorTask = null;
        }

        public void Dispose()
        {
            Results.Clear();
            Progress.Clear();
            _progressTask = null;
            _cancellationTokenSource.Cancel();
            _generatorTask = null;
            _serviceScope.Dispose();
        }
    }
}
