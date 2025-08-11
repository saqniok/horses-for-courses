using HorsesForCourses.Core;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.WebApi.Data
{
    public class EFCoachRepository : ICoachRepository
    {
        private readonly AppDbContext _context;

        // private static int _nextId = 1;

        public EFCoachRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Coach coach)
        {
            // coach.Id = _nextId++;
            await _context.Coaches.AddAsync(coach);
        }

        public async Task<Coach?> GetByIdAsync(int id)
        {
            return await _context.Coaches
                .Include(c => c.AssignedCourses)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Coach>> GetAllAsync()
        {
            return await _context.Coaches.ToListAsync();
        }

        public void Remove(int id)
        {
            var coach = _context.Coaches.Find(id);

            if (coach == null)
                return;

            _context.Coaches.Remove(coach);
        }

        public void Clear()
        {
            _context.Coaches.RemoveRange(_context.Coaches);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(Coach coach)
        {
            _context.Coaches.Update(coach);
        }
    }
}
