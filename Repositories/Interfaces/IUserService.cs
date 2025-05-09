using MeterChangeApi.Models;

namespace MeterChangeApi.Repositories.Interfaces
{
    /// <summary>
    /// Defines the contract for a service that handles user-related operations such as registration and authentication.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Registers a new user with the provided username, password, and optional email.
        /// </summary>
        /// <param name="username">The desired username for the new user.</param>
        /// <param name="password">The password for the new user.</param>
        /// <param name="email">The optional email address for the new user.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result is true if the registration was successful, otherwise false.</returns>
        Task<bool> RegisterUserAsync(string username, string password, string? email);

        /// <summary>
        /// Authenticates a user based on the provided username and password.
        /// </summary>
        /// <param name="username">The username of the user to authenticate.</param>
        /// <param name="password">The password to authenticate with.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains the <see cref="Users"/> entity if authentication is successful, otherwise null.</returns>
        Task<Users?> AuthenticateUserAsync(string username, string password);
    }
}