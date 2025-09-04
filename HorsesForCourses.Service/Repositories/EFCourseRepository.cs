using HorsesForCourses.Core;
using HorsesForCourses.Service.Data;
using HorsesForCourses.Service.DTOs;
using HorsesForCourses.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Service.Repositories
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
            return await _context.Courses
                .Include(c => c.AssignedCoach)
                .Include(c => c.Schedule)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CourseDto?> GetDtoByIdAsync(int id)
        {
            return await _context.Courses
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new CourseDto(
                    c.Id,
                    c.Title,
                    c.Period.StartDate,
                    c.Period.EndDate,
                    c.RequiredSkills.ToList(),
                    c.Schedule.Select(ts => new TimeSlotDto { Day = ts.Day, Start = ts.Start, End = ts.End }).ToList(),
                    c.IsConfirmed,
                    c.AssignedCoach != null ? new CoachShortDto(c.AssignedCoach.Id, c.AssignedCoach.Name) : null))
                .FirstOrDefaultAsync();
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
                .OrderBy(c => c.Id)
                .ToPagedResultAsync(request, ct);
        }

        public void Clear()
        {
            _context.Courses.RemoveRange(_context.Courses);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(Course course)
        {
            _context.Courses.Update(course);
        }

        public async Task DeleteAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
            }
        }
    }
}