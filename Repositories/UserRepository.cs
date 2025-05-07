using MeterChangeApi.Data;
using MeterChangeApi.Models;
using MeterChangeApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeterChangeApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ChangeOutContext _context;

        public UserRepository(ChangeOutContext context)
        {
            _context = context;
        }

        public async Task<Users?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task AddUserAsync(Users user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
    }
}
