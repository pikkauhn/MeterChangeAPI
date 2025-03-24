using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeterChangeApi.Data.Logger
{
    public class AppLogger : IAppLogger
    {
        private readonly ILogger _logger;

        public AppLogger(ILogger logger)
        {
            _logger = logger;
        }
        public void LogError(Exception ex, string message, params object[] args)
        {
            _logger.LogError(ex, message, args);
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }
    }
}