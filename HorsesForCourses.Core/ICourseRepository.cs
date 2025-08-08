
namespace HorsesForCourses.Core;

public interface ICourseRepository
{
    void Add(Course course);
    Course? GetById(int id);
    IEnumerable<Course> GetAll();
    void Clear();
    void SaveChanges();
    }

