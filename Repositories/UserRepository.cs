using MeterChangeApi.Data;
using MeterChangeApi.Models;
using MeterChangeApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeterChangeApi.Repositories
{
    /// <summary>
    /// Implements the <see cref="IUserRepository"/> interface to provide data access
    /// for <see cref="Users"/> entities using Entity Framework Core.
    /// </summary>
    /// <param name="context">The database context for the application.</param>
    public class UserRepository(ChangeOutContext context) : IUserRepository
    {
        private readonly ChangeOutContext _context = context;

        /// <inheritdoc />
        public async Task<Users?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        /// <inheritdoc />
        public async Task AddUserAsync(Users user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
    }
}