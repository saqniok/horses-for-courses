using HorsesForCourses.Core;
using HorsesForCourses.WebApi.DTOs;
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
         //   coach.Id = _nextId++;
            await _context.Coaches.AddAsync(coach);
        }

        public async Task<Coach?> GetByIdAsync(int id)
        {
            return await _context.Coaches
                .Include(c => c.AssignedCourses)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<CoachSummaryResponse>> GetAllAsync()
        {
            return await _context.Coaches
                .AsNoTracking()
                .OrderBy(p => p.Name).ThenBy(p => p.Id)
                .Select(p => new CoachSummaryResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Email = p.Email,
                    NumberOfCoursesAssignedTo = p.AssignedCourses.Count
                })
                .ToListAsync();
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

        public async Task DeleteAsync(int id)
        {
            var coach = await _context.Coaches.FindAsync(id);
            if (coach != null)
            {
                _context.Coaches.Remove(coach);
            }
        }

        public async Task<CoachDetailsDto?> GetDtoByIdAsync(int id)
        {
            return await _context.Coaches
                .AsNoTracking()
                .Select(c => new CoachDetailsDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    Skills = c.Skills.ToList(),
                    Courses = c.AssignedCourses.Select(ac => new CourseShortDto { Id = ac.Id, Title = ac.Title }).ToList()
                })
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}
