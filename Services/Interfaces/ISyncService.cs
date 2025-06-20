using MeterChangeApi.Models.DTOs;

namespace MeterChangeApi.Services.Interfaces
{
    public interface ISyncService
    {
        Task StreamSyncDataWithProgress(
            HttpResponse response,
            ILogger logger,
            CancellationToken cancellationToken = default);
    }
}
