using Serilog;

namespace DraftKings.LineupGenerator.Razor.Logging
{
    public static class LoggingConfiguration
    {
        public static LoggerConfiguration ConfigureRazorLogging(this LoggerConfiguration configuration)
        {
            configuration.WriteTo.Sink(new InMemorySink(20));

            return configuration;
        }
    }
}
