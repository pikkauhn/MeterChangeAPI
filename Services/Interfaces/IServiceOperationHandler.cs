namespace MeterChangeApi.Services.Interfaces
{
    public interface IServiceOperationHandler
    {
        Task<T> ExecuteServiceOperationAsync<T>(Func<Task<T>> operation, string errorMessage);
        Task ExecuteServiceOperationAsync(Func<Task> operation, string errorMessage);
    }
}