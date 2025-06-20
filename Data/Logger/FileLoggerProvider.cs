namespace MeterChangeApi.Data.Logger
{
    /// <summary>
    /// Provides instances of <see cref="FileLogger"/> to the logging system, writing logs to a specified file.
    /// Implements <see cref="ILoggerProvider"/> to integrate with the ASP.NET Core logging framework.
    /// Also implements <see cref="IDisposable"/> for resource management.
    /// </summary>
    /// <param name="filePath">The path to the log file where logs will be written.</param>
    public class FileLoggerProvider(string filePath) : ILoggerProvider, IDisposable
    {
        private readonly string _filePath = filePath;

        /// <summary>
        /// Creates a new <see cref="ILogger"/> instance of the <see cref="FileLogger"/> type.
        /// </summary>
        /// <param name="categoryName">The category name for the logger.</param>
        /// <returns>A new instance of <see cref="FileLogger"/>.</returns>
        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(_filePath);
        }

        /// <summary>
        /// Disposes of any resources used by the provider. In this case, there are no managed resources to dispose of explicitly.
        /// The call to <see cref="GC.SuppressFinalize(object)"/> prevents the finalizer from being called.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// A simple file logger that writes log messages to a specified file.
        /// This is a nested class within <see cref="FileLoggerProvider"/>.
        /// </summary>
        /// <param name="filePath">The path to the log file where logs will be written.</param>
        private class FileLogger(string filePath) : ILogger
        {
            private readonly string _filePath = filePath;

            /// <inheritdoc />
            IDisposable? ILogger.BeginScope<TState>(TState state)
            {
                // Scoping is not implemented for this simple file logger.
                return null;
            }

            /// <inheritdoc />
            public bool IsEnabled(LogLevel logLevel)
            {
                // Log all levels for this file logger. In a more advanced scenario,
                // you might configure which log levels are enabled.
                return true;
            }

            /// <inheritdoc />
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                try
                {
                    // Append the log message to the specified file. Using 'using' ensures the file is closed.
                    using var writer = File.AppendText(_filePath);
                    writer.WriteLine($"{DateTime.Now} [{logLevel}] {formatter(state, exception)}");

                    // If there's an exception, write its details to the log file as well.
                    if (exception != null)
                    {
                        writer.WriteLine(exception.ToString());
                    }
                }
                catch (Exception ex)
                {
                    // Handle any errors that might occur during the logging process itself.
                    // Writing to the console is a basic fallback. A more robust solution might involve
                    // logging to a different sink or implementing retry logic.
                    Console.WriteLine($"Error writing to log file: {ex.Message}");
                }
            }
        }
    }
}