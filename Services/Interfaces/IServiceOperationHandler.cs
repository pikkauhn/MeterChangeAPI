namespace MeterChangeApi.Services.Interfaces
{
    /// <summary>
    /// Defines a contract for handling the execution of service operations with consistent error handling and logging.
    /// </summary>
    public interface IServiceOperationHandler
    {
        /// <summary>
        /// Executes an asynchronous service operation that returns a result. Catches any exceptions and throws a <see cref="ServiceOperationException"/> with the provided error message.
        /// </summary>
        /// <typeparam name="T">The type of the result returned by the operation.</typeparam>
        /// <param name="operation">A function that represents the asynchronous service operation to execute.</param>
        /// <param name="errorMessage">The error message to include in the <see cref="ServiceOperationException"/> if an exception occurs.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains the result of the service operation.</returns>
        /// <exception cref="ServiceOperationException">Thrown when an exception occurs during the execution of the service operation.</exception>
        Task<T> ExecuteServiceOperationAsync<T>(Func<Task<T>> operation, string errorMessage);

        /// <summary>
        /// Executes an asynchronous service operation that does not return a result. Catches any exceptions and throws a <see cref="ServiceOperationException"/> with the provided error message.
        /// </summary>
        /// <param name="operation">A function that represents the asynchronous service operation to execute.</param>
        /// <param name="errorMessage">The error message to include in the <see cref="ServiceOperationException"/> if an exception occurs.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="ServiceOperationException">Thrown when an exception occurs during the execution of the service operation.</exception>
        Task ExecuteServiceOperationAsync(Func<Task> operation, string errorMessage);
    }
}