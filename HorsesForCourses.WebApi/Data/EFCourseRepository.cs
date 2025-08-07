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
            _context.SaveChanges();
        }

        public Course? GetById(int id)
        {
            return _context.Courses.Include(c => c.Schedule).FirstOrDefault(c => c.Id == id);
        }

        public IEnumerable<Course> GetAll()
        {
            return _context.Courses.Include(c => c.Schedule).ToList();
        }
    }
}

