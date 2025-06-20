namespace MeterChangeApi.Data.Logger
{
    /// <summary>
    /// Defines an interface for application-specific logging, abstracting away the underlying logging implementation.
    /// This interface encourages the use of message templates for structured logging.
    /// </summary>
    public interface IAppLogger
    {
        /// <summary>
        /// Logs an error message and an associated exception.
        /// </summary>
        /// <param name="ex">The exception that occurred.</param>
        /// <param name="template">The message template format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format into the template.</param>
        void LogError(Exception ex, string template, params object[] args);

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="template">The message template format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format into the template.</param>
        void LogInformation(string template, params object[] args);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="template">The message template format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format into the template.</param>
        void LogWarning(string template, params object[] args);
    }
}