using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;

namespace DraftKings.LineupGenerator.Business.Logging
{
    public static class SerilogConfiguration
    {
        private const string MetricsNamespace = "DraftKings.LineupGenerator.Business.Metrics";

        public static LoggerConfiguration Build(bool logToConsole = true, bool logToFile = true, Action<LoggerConfiguration> configure = null)
        {
            var configuration = new LoggerConfiguration();

            if (logToFile)
            {
                AddFileSink(configuration);
            }

            if (logToConsole)
            {
                AddConsoleSink(configuration);
            }

            configure?.Invoke(configuration);

            return configuration;
        }

        private static void AddFileSink(LoggerConfiguration configuration)
        {
            configuration.WriteTo.Logger(subLogger =>
            {
                subLogger.Filter.ByIncludingOnly(logEvent =>
                {
                    return logEvent.Properties.TryGetValue("SourceContext", out var logEventProperty) &&
                        logEventProperty is ScalarValue scalarValue &&
                        scalarValue.Value.ToString().StartsWith(MetricsNamespace);
                });

                subLogger.WriteTo.Async(wt => wt.File(
                    "./logs/metrics.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 1,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{correlationId}] [{Level:u3}] {Message:lj}{NewLine}{Exception}"));
            });

            configuration.WriteTo.Logger(subLogger =>
            {
                subLogger.Filter.ByExcluding(logEvent =>
                {
                    return logEvent.Properties.TryGetValue("SourceContext", out var logEventProperty) &&
                        logEventProperty is ScalarValue scalarValue &&
                        scalarValue.Value.ToString().StartsWith(MetricsNamespace);
                });

                subLogger.WriteTo.Async(wt => wt.File(
                    "./logs/log.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 1,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{correlationId}] [{Level:u3}] {Message:lj}{NewLine}{Exception}"));
            });
        }

        private static void AddConsoleSink(LoggerConfiguration configuration)
        {
            configuration.WriteTo.Logger(subLogger =>
            {
                subLogger.Filter.ByIncludingOnly(logEvent =>
                {
                    if (!logEvent.Properties.ContainsKey("SourceContext"))
                    {
                        return false;
                    }

                    var logEventProperty = logEvent.Properties["SourceContext"];

                    return logEventProperty is ScalarValue scalarValue &&
                        scalarValue.Value.ToString().StartsWith("DraftKings") &&
                        !scalarValue.Value.ToString().StartsWith(MetricsNamespace);
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
