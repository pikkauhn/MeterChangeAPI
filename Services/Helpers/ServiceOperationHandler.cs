using MeterChangeApi.Services.Interfaces;
using MeterChangeApi.Middleware.ExceptionHandling;

namespace MeterChangeApi.Services.Helpers
{
    public class ServiceOperationHandler : IServiceOperationHandler
    {
        public async Task<T> ExecuteServiceOperationAsync<T>(Func<Task<T>> operation, string errorMessage)
        {
            try
            {
                return await operation();
            }
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
            catch (Exception ex)
            {
                throw new ServiceException($"Error: {errorMessage}", ex);
            }
        }

        public async Task ExecuteServiceOperationAsync(Func<Task> operation, string errorMessage)
        {
            try
            {
                await operation();
            }
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
            catch (Exception ex)
            {
                throw new ServiceException($"Error: {errorMessage}", ex);
            }
        }
    }
}