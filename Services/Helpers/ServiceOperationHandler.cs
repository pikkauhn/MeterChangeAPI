using MeterChangeApi.Middleware.ExceptionHandling;
using MeterChangeApi.Services.Interfaces;

namespace MeterChangeApi.Services.Helpers
{
    /// <summary>
    /// A helper class that implements <see cref="IServiceOperationHandler"/> to provide consistent
    /// execution and exception handling for service operations.
    /// </summary>
    public class ServiceOperationHandler : IServiceOperationHandler
    {
        /// <inheritdoc />
        public async Task<T> ExecuteServiceOperationAsync<T>(Func<Task<T>> operation, string errorMessage)
        {
            try
            {
                return await operation();
            }
            // Re-throw specific exceptions without wrapping them.
            catch (NotFoundException)
            {
                throw;
            }
            catch (InvalidInputException)
            {
                throw;
            }
            catch (DatabaseConflictException)
            {
                throw;
            }
            // Wrap any other exceptions in a ServiceException to provide a consistent error type at the service level.
            catch (Exception ex)
            {
                throw new ServiceException($"Error: {errorMessage}", ex);
            }
        }

        /// <inheritdoc />
        public async Task ExecuteServiceOperationAsync(Func<Task> operation, string errorMessage)
        {
            try
            {
                await operation();
            }
            // Re-throw specific exceptions without wrapping them.
            catch (NotFoundException)
            {
                throw;
            }
            catch (InvalidInputException)
            {
                throw;
            }
            catch (DatabaseConflictException)
            {
                throw;
            }
            // Wrap any other exceptions in a ServiceException to provide a consistent error type at the service level.
            catch (Exception ex)
            {
                throw new ServiceException($"Error: {errorMessage}", ex);
            }
        }
    }
}