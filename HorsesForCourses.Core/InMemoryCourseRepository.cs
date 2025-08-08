using HorsesForCourses.Core;

public class InMemoryCourseRepository : ICourseRepository
{
    private readonly List<Course> _courses = new();
    private int _nextId = 1;

    public void Add(Course course)
    {
        course.Id = _nextId++;
        _courses.Add(course);
    }

    public IEnumerable<Course> GetAll() => _courses;

    public Course? GetById(int id) => _courses.FirstOrDefault(c => c.Id == id);


    public void Clear() => _courses.Clear();
}
