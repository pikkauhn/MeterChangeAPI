using Microsoft.EntityFrameworkCore;
using MeterChangeApi.Middleware.ExceptionHandling;
using MySql.Data.MySqlClient;
using MeterChangeApi.Repositories.Interfaces;

namespace MeterChangeApi.Repositories.Helpers
{
    /// <summary>
    /// A helper class that implements <see cref="IDatabaseOperationHandler"/> to provide consistent
    /// execution and exception handling for database operations. It specifically handles
    /// <see cref="DbUpdateException"/> for MySQL duplicate key errors.
    /// </summary>
    public class DatabaseOperationHandler : IDatabaseOperationHandler
    {
        /// <inheritdoc />
        public async Task<T> ExecuteDbOperationAsync<T>(Func<Task<T>> operation, string errorMessage)
        {
            try
            {
                return await operation();
            }
            // Catch DbUpdateException for MySQL duplicate key errors (Error Number 1062).
            catch (DbUpdateException ex) when (ex.InnerException is MySqlException mySqlEx && mySqlEx.Number == 1062)
            {
                throw new DatabaseConflictException(errorMessage, ex);
            }
            // Catch other DbUpdateExceptions, likely related to data integrity or database constraints.
            catch (DbUpdateException ex)
            {
                throw new RepositoryException(errorMessage, ex);
            }
            // Catch any other unexpected exceptions during database operations.
            catch (Exception ex)
            {
                throw new RepositoryException($"Unexpected error during database operation: {errorMessage}", ex);
            }
        }

        /// <inheritdoc />
        public async Task ExecuteDbOperationAsync(Func<Task> operation, string errorMessage)
        {
            try
            {
                await operation();
            }
            // Catch DbUpdateException for MySQL duplicate key errors (Error Number 1062).
            catch (DbUpdateException ex) when (ex.InnerException is MySqlException mySqlEx && mySqlEx.Number == 1062)
            {
                throw new DatabaseConflictException(errorMessage, ex);
            }
            // Catch other DbUpdateExceptions, likely related to data integrity or database constraints.
            catch (DbUpdateException ex)
            {
                throw new RepositoryException(errorMessage, ex);
            }
            // Catch any other unexpected exceptions during database operations.
            catch (Exception ex)
            {
                throw new RepositoryException($"Unexpected error during database operation: {errorMessage}", ex);
            }
        }
    }
}