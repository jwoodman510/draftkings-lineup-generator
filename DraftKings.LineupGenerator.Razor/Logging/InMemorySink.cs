using Serilog.Core;
using Serilog.Events;
using System.Collections.Concurrent;

namespace DraftKings.LineupGenerator.Razor.Logging
{
    public class InMemorySink : ILogEventSink
    {
        public static IReadOnlyCollection<string> LogEvents => _logEvents;

        private readonly int _queueLimit;
        private static readonly ConcurrentQueue<string> _logEvents = new();

        public InMemorySink(int queueLimit)
        {
            _queueLimit = queueLimit;
        }

        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage();
            
            if (logEvent.Properties.ContainsKey("Time"))
            {
                _logEvents.Enqueue(message);
            }
            else
            {
                _logEvents.Enqueue($"[{DateTime.Now:HH:mm:ss}] {message}");
            }

            if (_logEvents.Count >= _queueLimit)
            {
                _logEvents.TryDequeue(out _);
            }
        }
    }
}
