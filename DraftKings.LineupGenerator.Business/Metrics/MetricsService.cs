using DraftKings.LineupGenerator.Business.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace DraftKings.LineupGenerator.Business.Metrics
{
    public class MetricsService : IMetricsService, IDisposable
    {
        private readonly ILogger<MetricsService> _logger;
        private readonly ConcurrentDictionary<string, Stopwatch> _actions = new ConcurrentDictionary<string, Stopwatch>();
        private readonly ConcurrentDictionary<string, ActionAverage> _actionAverages = new ConcurrentDictionary<string, ActionAverage>();

        public MetricsService(ILogger<MetricsService> logger)
        {
            _logger = logger;
        }

        public void StartAction(string action)
        {
            _actions.TryAdd(action, Stopwatch.StartNew());
        }

        public void EndAction(string action)
        {
            if (_actions.TryRemove(action, out Stopwatch stopwatch))
            {
                stopwatch.Stop();
                LogAction(action, stopwatch.Elapsed);
            }
        }

        public void LogAction(string action, TimeSpan elapsed)
        {
            _logger.LogInformation("{Action} took {Duration}", action, elapsed);

            if (!_actionAverages.TryGetValue(action, out var actionAverage))
            {
                _actionAverages[action] = new ActionAverage(elapsed);

                return;
            }

            actionAverage.Update(elapsed);

            _logger.LogInformation("{Action} | Average: {Average} | Count: {Count}", action, actionAverage.Average, actionAverage.Count);
        }

        public void Dispose()
        {
            foreach (var stopwatch in _actions.Values)
            {
                stopwatch.Stop();
            }

            _actions.Clear();
        }

        private class ActionAverage
        {
            public int Count { get; private set; }
            public double AverageMilliseconds { get; private set; }
            public TimeSpan Average => TimeSpan.FromMilliseconds(AverageMilliseconds);

            public ActionAverage(TimeSpan elapsed)
            {
                Count = 1;
                AverageMilliseconds = elapsed.TotalMilliseconds;
            }

            public void Update(TimeSpan elapsed)
            {
                var updatedAverage = (Count * AverageMilliseconds + elapsed.TotalMilliseconds) / (Count + 1);

                ++Count;
                AverageMilliseconds = updatedAverage;
            }
        }
    }
}
