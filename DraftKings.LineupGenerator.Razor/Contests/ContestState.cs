using DraftKings.LineupGenerator.Business.Interfaces;
using DraftKings.LineupGenerator.Models.Contests;
using DraftKings.LineupGenerator.Models.Lineups;
using Microsoft.Extensions.DependencyInjection;

namespace DraftKings.LineupGenerator.Razor.Contests
{
    public class ContestState : IDisposable
    {
        public bool IsRunning => _generatorTask != null && !_cancellationTokenSource.IsCancellationRequested;

        public readonly Guid Id;

        public readonly ContestModel ContestModel;

        public readonly LineupRequestModel RequestModel;

        public List<LineupsModel> Results { get; private set; } = [];

        public event EventHandler<ContestState> ResultsChanged
        {
            add
            {
                _resultsChanged += value;
            }
            remove
            {
                _resultsChanged -= value;
            }
        }

        public List<(string generator, long iterationCount, long validLineupCount)> Progress { get; private set; } = [];

        private Task? _generatorTask;
        private Task? _progressTask;
        private EventHandler<ContestState>? _resultsChanged;
        private CancellationTokenSource _cancellationTokenSource = new();


        private readonly IServiceScope _serviceScope;
        private readonly ILineupGeneratorService _lineupGeneratorService;

        public ContestState(ContestModel contestModel, IServiceScope serviceScope)
        {
            _serviceScope = serviceScope;
            _lineupGeneratorService = serviceScope.ServiceProvider.GetRequiredService<ILineupGeneratorService>();

            Id = Guid.NewGuid();
            ContestModel = contestModel;
            RequestModel = new LineupRequestModel(contestModel.Id);
        }

        public ContestState(ContestHistoryModel contestHistoryModel, IServiceScope serviceScope)
        {
            _serviceScope = serviceScope;
            _lineupGeneratorService = serviceScope.ServiceProvider.GetRequiredService<ILineupGeneratorService>();

            Id = contestHistoryModel.Id;
            ContestModel = contestHistoryModel.ContestModel;
            RequestModel = contestHistoryModel.RequestModel;
            Results = contestHistoryModel.Results;
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
                var results = await _lineupGeneratorService.GetAsync(RequestModel, _cancellationTokenSource.Token);

                _cancellationTokenSource.Cancel();

                Results = results;
                _resultsChanged?.Invoke(this, this);
            }, TaskCreationOptions.LongRunning);

            _progressTask = Task.Factory.StartNew(async () =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    Progress = _lineupGeneratorService.GetProgress().ToList();
                    Results = _lineupGeneratorService.GetCurrentLineups().ToList();
                    _resultsChanged?.Invoke(this, this);

                    await Task.Delay(TimeSpan.FromSeconds(10), _cancellationTokenSource.Token);
                }
            });
        }

        public void Cancel()
        {
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
