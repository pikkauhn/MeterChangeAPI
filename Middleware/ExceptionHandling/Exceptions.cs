namespace MeterChangeApi.Middleware.ExceptionHandling
{
    /// <summary>
    /// Custom exception class representing an error that occurred within the data repository layer.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public class RepositoryException(string message, Exception innerException) : Exception(message, innerException)
    {
    }

    /// <summary>
    /// Custom exception class representing an error that occurred within the service layer.
    /// </summary>
    public class ServiceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public ServiceException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public ServiceException(string message) : base(message) { }
    }

    /// <summary>
    /// Custom exception class representing a "Not Found" error, typically when a requested resource could not be located.
    /// </summary>
    /// <param name="message">The error message that describes what was not found.</param>
    public class NotFoundException(string message) : Exception(message)
    {
    }

    /// <summary>
    /// Custom exception class representing a "Database Conflict" error, typically when an operation violates a database constraint (e.g., a duplicate entry).
    /// </summary>
    /// <param name="message">The error message that explains the reason for the conflict.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public class DatabaseConflictException(string message, Exception innerException) : Exception(message, innerException)
    {
    }

    /// <summary>
    /// Custom exception class representing an error due to invalid input provided by the client.
    /// </summary>
    /// <param name="message">The error message that describes the invalid input.</param>
    public class InvalidInputException(string message) : Exception(message)
    {
    }
}