using HorsesForCourses.Core;
using HorsesForCourses.Service.Data;
using HorsesForCourses.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Service.Repositories
{
    public class EFUserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public EFUserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}