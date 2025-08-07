

using System.Collections.Generic;

namespace HorsesForCourses.Core
{
    public interface ICourseRepository
    {
        void Add(Course course);
        Course? GetById(int id);
        IEnumerable<Course> GetAll();
    }
}

