using MeterChangeApi.Models;

namespace MeterChangeApi.Repositories.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(string username, string password, string? email);
        Task<Users?> AuthenticateUserAsync(string username, string password);
    }
}
