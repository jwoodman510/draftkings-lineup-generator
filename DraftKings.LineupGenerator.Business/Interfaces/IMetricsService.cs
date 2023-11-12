using System;

namespace DraftKings.LineupGenerator.Business.Interfaces
{
    public interface IMetricsService
    {
        void StartAction(string action);

        void EndAction(string action);

        void LogAction(string action, TimeSpan elapsed);
    }
}
