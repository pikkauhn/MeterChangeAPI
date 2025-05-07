using MeterChangeApi.Models;

namespace MeterChangeApi.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<Users?> GetUserByUsernameAsync(string username);
        Task AddUserAsync(Users user);
        Task<bool> UserExistsAsync(string username);
    }
}
