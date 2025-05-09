using MeterChangeApi.Models;

namespace MeterChangeApi.Repositories.Interfaces
{
    /// <summary>
    /// Defines the contract for a repository that handles access to <see cref="Users"/> entities.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves a <see cref="Users"/> entity by their username.
        /// </summary>
        /// <param name="username">The username of the user to retrieve.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains the <see cref="Users"/> entity if found, otherwise null.</returns>
        Task<Users?> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Adds a new <see cref="Users"/> entity to the repository.
        /// </summary>
        /// <param name="user">The <see cref="Users"/> entity to add.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task AddUserAsync(Users user);

        /// <summary>
        /// Checks if a user with the given username exists in the repository.
        /// </summary>
        /// <param name="username">The username to check for.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result is true if a user with the username exists, otherwise false.</returns>
        Task<bool> UserExistsAsync(string username);
    }
}