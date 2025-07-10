namespace HorsesForCourses.Core;

public record Period 
public class Course
{
    public string Name { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }

    public List<Lesson> Lessons { get; private set; } = new();

}
