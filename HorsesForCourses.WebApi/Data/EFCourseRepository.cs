using HorsesForCourses.Core;
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

        public void Add(Course course)
        {
            _context.Courses.Add(course);
        }

        public Course? GetById(int id)
        {
            return _context.Courses.Include(c => c.AssignedCoach).FirstOrDefault(c => c.Id == id);
        }

        public IEnumerable<Course> GetAll()
        {
            return _context.Courses.Include(c => c.AssignedCoach).ToList();
        }

        public void Clear()
        {
            _context.Courses.RemoveRange(_context.Courses);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

    }
}

