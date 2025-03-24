namespace MeterChangeApi.Repositories.Interfaces

{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }

    public interface IDatabaseOperationHandler
    {
        Task<T> ExecuteDbOperationAsync<T>(Func<Task<T>> operation, string errorMessage);
        Task ExecuteDbOperationAsync(Func<Task> operation, string errorMessage);
    }
}