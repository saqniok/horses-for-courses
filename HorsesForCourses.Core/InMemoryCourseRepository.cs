using HorsesForCourses.Core;

public class InMemoryCourseRepository
{
    private readonly List<Course> _courses = new();

    public IEnumerable<Course> GetAll() => _courses;

    public Course? GetById(Guid id) => _courses.FirstOrDefault(c => c.Id == id);

    public void Add(Course course) => _courses.Add(course);

    public void Clear() => _courses.Clear();
}
