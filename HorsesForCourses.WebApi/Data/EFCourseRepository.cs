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
            return _context.Courses.FirstOrDefault(c => c.Id == id);
        }

        public IEnumerable<Course> GetAll()
        {
            return _context.Courses.ToList();
        }

        public void Clear()
        {
            _context.Courses.RemoveRange(_context.Courses);
        }

        // public void Update(Course course)
        // {
        //     _context.Courses.Update(course);
        //     _context.SaveChanges();
        // }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

    }
}

