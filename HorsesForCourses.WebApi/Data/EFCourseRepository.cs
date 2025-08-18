using HorsesForCourses.Core;
using HorsesForCourses.WebApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.WebApi.Data
{
    public class EFCourseRepository : ICourseRepository
    {
        private readonly AppDbContext _context;

        public EFCourseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
        }


        public async Task<Course?> GetByIdAsync(int id)
        {
            return await _context.Courses.Include(c => c.AssignedCoach).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<CourseDto>> GetAllAsync()
        {
            return await _context.Courses
            .AsNoTracking()
            .OrderBy(p => p.Title).ThenBy(p => p.Id)
            .Select(p => new CourseDto(
                p.Id,
                p.Title,
                p.Period.StartDate,
                p.Period.EndDate,
                p.RequiredSkills.ToList(),
                p.Schedule.Select(ts => new TimeSlotDto { Day = ts.Day, Start = ts.Start, End = ts.End }).ToList(),
                p.IsConfirmed,
                p.AssignedCoach != null ? new CoachShortDto(p.AssignedCoach.Id, p.AssignedCoach.Name) : null ))
            .ToListAsync();
        }

        public async Task<PagedResult<Course>> GetPagedAsync(PageRequest request, CancellationToken ct = default)
        {
            return await _context.Courses
                .Include(c => c.AssignedCoach)
                .OrderBy(c => c.Id)                // ⚠ обязательно сортируем
                .ToPagedResultAsync(request, ct); // используем твой helper
        }

        public void Clear()
        {
            _context.Courses.RemoveRange(_context.Courses); // This will clear all courses
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(Course course)
        {
            _context.Courses.Update(course);
        }

    }
}
