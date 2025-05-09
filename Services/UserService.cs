using MeterChangeApi.Models;
using MeterChangeApi.Repositories.Interfaces;

namespace MeterChangeApi.Services
{
    /// <summary>
    /// Implements the <see cref="IUserService"/> interface to handle user registration and authentication.
    /// </summary>
    /// <param name="userRepository">The repository for accessing user data.</param>
    public class UserService(IUserRepository userRepository) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;

        /// <summary>
        /// Registers a new user with the provided username, password, and optional email.
        /// Hashes the password before storing it in the database.
        /// </summary>
        /// <param name="username">The username for the new user.</param>
        /// <param name="password">The password for the new user (will be hashed).</param>
        /// <param name="email">The optional email address for the new user.</param>
        /// <returns>True if the registration was successful, false if the username already exists.</returns>
        public async Task<bool> RegisterUserAsync(string username, string password, string? email)
        {
            // Check if a user with the given username already exists.
            if (await _userRepository.UserExistsAsync(username))
            {
                return false; // Username already taken.
            }

            // Hash the user's password using BCrypt.
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            // Create a new Users object with the provided information.
            var newUser = new Users
            {
                Username = username,
                PasswordHash = passwordHash,
                Email = email,
                RegistrationDate = DateTime.UtcNow // Record the registration timestamp.
            };

            // Add the new user to the database via the repository.
            await _userRepository.AddUserAsync(newUser);
            return true; // Registration successful.
        }

        /// <summary>
        /// Authenticates a user based on the provided username and password.
        /// Retrieves the user by username and then verifies the password hash.
        /// </summary>
        /// <param name="username">The username of the user to authenticate.</param>
        /// <param name="password">The password to verify.</param>
        /// <returns>The <see cref="Users"/> object if authentication is successful, otherwise null.</returns>
        public async Task<Users?> AuthenticateUserAsync(string username, string password)
        {
            // Retrieve the user from the database based on the provided username.
            var user = await _userRepository.GetUserByUsernameAsync(username);

            // If a user is found, verify the provided password against the stored password hash.
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return user; // Authentication successful.
            }

            return null; // Authentication failed (user not found or password incorrect).
        }
    }
}