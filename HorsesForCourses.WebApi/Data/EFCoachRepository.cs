using HorsesForCourses.Core;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.WebApi.Data
{
    public class EFCoachRepository : ICoachRepository
    {
        private readonly AppDbContext _context;

        public EFCoachRepository(AppDbContext context)
        {
            _context = context;
        }

        async void AddAsync(Coach coach)
        {
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

        Task ICoachRepository.AddAsync(Coach coach)
        {
            throw new NotImplementedException();
        }

        public void Remove(Coach coach)
        {
            throw new NotImplementedException();
        }
    }
}
