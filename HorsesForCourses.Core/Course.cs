namespace HorsesForCourses.Core;


public class Course
{
    public string Name { get; private set; }
    public List<Lesson> Lessons { get; private set; } = new();

}
