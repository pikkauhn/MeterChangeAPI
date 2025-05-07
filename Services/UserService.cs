using MeterChangeApi.Models;
using MeterChangeApi.Repositories.Interfaces;

namespace MeterChangeApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> RegisterUserAsync(string username, string password, string? email)
        {
            if (await _userRepository.UserExistsAsync(username))
            {
                return false; // Username already exists
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new Users
            {
                Username = username,
                PasswordHash = passwordHash,
                Email = email,
                RegistrationDate = DateTime.UtcNow
            };

            await _userRepository.AddUserAsync(newUser);
            return true;
        }

        public async Task<Users?> AuthenticateUserAsync(string username, string password)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return user;
            }

            return null; // Authentication failed
        }
    }
}