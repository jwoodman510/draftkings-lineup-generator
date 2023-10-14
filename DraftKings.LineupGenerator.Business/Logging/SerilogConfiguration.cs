using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace DraftKings.LineupGenerator.Business.Logging
{
    public static class SerilogConfiguration
    {
        public static LoggerConfiguration Build()
        {
            var configuration = new LoggerConfiguration();

            AddFileSink(configuration);
            AddConsoleSink(configuration);

            return configuration;
        }

        private static void AddFileSink(LoggerConfiguration configuration)
        {
            configuration.WriteTo.Async(wt => wt.File(
                "./logs/log.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 1,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{correlationId}] [{Level:u3}] {Message:lj}{NewLine}{Exception}"));
        }

        private static void AddConsoleSink(LoggerConfiguration configuration)
        {
            configuration.WriteTo.Logger(subLogger =>
            {
                subLogger.Filter.ByIncludingOnly(logEvent =>
                {
                    return logEvent.Properties.TryGetValue("SourceContext", out var logEventProperty) &&
                        logEventProperty is ScalarValue scalarValue &&
                        scalarValue.Value.ToString().StartsWith("DraftKings");
                });

                subLogger.WriteTo.Async(sink =>
                {
                    sink.Console(
                        LogEventLevel.Information,
                        "{Message}{NewLine}",
                        theme: SystemConsoleTheme.Literate);
                });
            });
        }
    }
}
