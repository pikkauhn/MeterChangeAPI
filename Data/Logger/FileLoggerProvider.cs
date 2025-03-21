public class FileLoggerProvider : ILoggerProvider, IDisposable
{
    private readonly string _filePath;

    public FileLoggerProvider(string filePath)
    {
        _filePath = filePath;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(_filePath);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this); // Suppress finalization
    }

    private class FileLogger : ILogger
    {
        private readonly string _filePath;

        public FileLogger(string filePath)
        {
            _filePath = filePath;
        }

        IDisposable? ILogger.BeginScope<TState>(TState state)
        {
            return null; // Scoping not implemented.
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true; // Log all levels
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            try
            {
                using (var writer = File.AppendText(_filePath))
                {
                    writer.WriteLine($"{DateTime.Now} [{logLevel}] {formatter(state, exception)}");
                    if (exception != null)
                    {
                        writer.WriteLine(exception.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle logging errors, e.g., to the console
                Console.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }
    }
}