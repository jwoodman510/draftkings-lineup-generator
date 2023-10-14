using Microsoft.Extensions.Logging;
using System;

namespace DraftKings.LineupGenerator.Business.Extensions
{
    public static class ILoggerExtensions
    {
        public static void LogInformationGreen(this ILogger logger, string message)
        {
            // Trick serilog into thinking this is a scalar value
            try
            {
                logger.LogInformation("{0}", new Uri(message, UriKind.Relative));
            }
            catch
            {
                logger.LogInformation(message);
            }            
        }
    }
}
