using Microsoft.EntityFrameworkCore;
using MeterChangeApi.Middleware.ExceptionHandling;
using MySql.Data.MySqlClient;
using MeterChangeApi.Repositories.Interfaces;

namespace MeterChangeApi.Repositories.Helpers
{
    public class DatabaseOperationHandler : IDatabaseOperationHandler
    {
        public async Task<T> ExecuteDbOperationAsync<T>(Func<Task<T>> operation, string errorMessage)
        {
            try
            {
                return await operation();
            }
            catch (DbUpdateException ex) when (ex.InnerException is MySqlException mySqlEx && mySqlEx.Number == 1062)
            {
                throw new DatabaseConflictException(errorMessage, ex);
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException(errorMessage, ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Unexpected error: {errorMessage}", ex);
            }
        }

        public async Task ExecuteDbOperationAsync(Func<Task> operation, string errorMessage)
        {
            try
            {
                await operation();
            }
            catch (DbUpdateException ex) when (ex.InnerException is MySqlException mySqlEx && mySqlEx.Number == 1062)
            {
                throw new DatabaseConflictException(errorMessage, ex);
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException(errorMessage, ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Unexpected error: {errorMessage}", ex);
            }
        }
    }
}