namespace MeterChangeApi.Data.Logger
{
    /// <summary>
    /// Implements the <see cref="IAppLogger"/> interface, providing a wrapper around the standard <see cref="ILogger"/>.
    /// </summary>
    /// <param name="logger">The instance of the generic <see cref="ILogger"/> to use for logging.</param>
    public class AppLogger(ILogger logger) : IAppLogger
    {
        private readonly ILogger _logger = logger;

        /// <inheritdoc />
        public void LogError(Exception ex, string template, params object[] args) => _logger.LogError(ex, template, args);

        /// <inheritdoc />
        public void LogInformation(string template, params object[] args) => _logger.LogInformation(template, args);

        /// <inheritdoc />
        public void LogWarning(string template, params object[] args) => _logger.LogWarning(template, args);
    }
}