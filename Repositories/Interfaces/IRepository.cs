namespace MeterChangeApi.Repositories.Interfaces
{
    /// <summary>
    /// Defines a generic contract for a repository that handles basic CRUD (Create, Read, Update, Delete) operations
    /// for entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the entity that the repository manages. Must be a class.</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Retrieves an entity of type <typeparamref name="T"/> by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the entity to retrieve.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains the entity if found, otherwise null.</returns>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves all entities of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains a collection of all entities.</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Adds a new entity of type <typeparamref name="T"/> to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task AddAsync(T entity);

        /// <summary>
        /// Updates an existing entity of type <typeparamref name="T"/> in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Deletes an entity of type <typeparamref name="T"/> from the repository by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the entity to delete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task DeleteAsync(int id);
    }

    /// <summary>
    /// Defines a contract for handling the execution of database operations with consistent error handling and logging.
    /// </summary>
    public interface IDatabaseOperationHandler
    {
        /// <summary>
        /// Executes an asynchronous database operation that returns a result. Catches any exceptions and throws a <see cref="DatabaseOperationException"/> with the provided error message.
        /// </summary>
        /// <typeparam name="T">The type of the result returned by the operation.</typeparam>
        /// <param name="operation">A function that represents the asynchronous database operation to execute.</param>
        /// <param name="errorMessage">The error message to include in the <see cref="DatabaseOperationException"/> if an exception occurs.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains the result of the database operation.</returns>
        /// <exception cref="DatabaseOperationException">Thrown when an exception occurs during the execution of the database operation.</exception>
        Task<T> ExecuteDbOperationAsync<T>(Func<Task<T>> operation, string errorMessage);

        /// <summary>
        /// Executes an asynchronous database operation that does not return a result. Catches any exceptions and throws a <see cref="DatabaseOperationException"/> with the provided error message.
        /// </summary>
        /// <param name="operation">A function that represents the asynchronous database operation to execute.</param>
        /// <param name="errorMessage">The error message to include in the <see cref="DatabaseOperationException"/> if an exception occurs.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="DatabaseOperationException">Thrown when an exception occurs during the execution of the database operation.</exception>
        Task ExecuteDbOperationAsync(Func<Task> operation, string errorMessage);
    }
}