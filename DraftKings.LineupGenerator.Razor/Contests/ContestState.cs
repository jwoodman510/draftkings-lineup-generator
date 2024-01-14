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

        public ContestSearchModel SearchModel { get; set; } = default!;

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

                SetResults(results);
            }, TaskCreationOptions.LongRunning);

            _progressTask = Task.Factory.StartNew(async () =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    Progress = _lineupGeneratorService.GetProgress().ToList();
                    var results = _lineupGeneratorService.GetCurrentLineups().ToList();
                    SetResults(results);

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

        private void SetResults(List<LineupsModel> results)
        {            
            foreach (var result in results)
            {
                result.Lineups = result.Lineups.Take(RequestModel.LineupCount).ToList();
            }

            Results = results;

            _resultsChanged?.Invoke(this, this);
        }
    }
}
