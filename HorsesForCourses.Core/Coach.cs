namespace HorsesForCourses.Core;


public class Coach
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public HashSet<string> Skills { get; private set; } = new();
    public List<Course> AssignedCourses { get; private set; } = new();

    // Для EF
    protected Coach() { }

    public Coach(string name, string email)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Skills = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        AssignedCourses = new List<Course>();
    }

    public void AddSkill(string skill)
    {
        if (Skills.Contains(skill.ToLower()))
            throw new ArgumentException("Skill already added");

        Skills.Add(skill);
    }

    public void UpdateSkills(IEnumerable<string> newSkills)
    {
        Skills.Clear();
        newSkills.ToList().ForEach(skill => Skills.Add((skill)));
    }

    public void UpdateDetails(string name, string email)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
    }

    public bool HasAllSkills(IEnumerable<string> requiredSkills)
        => requiredSkills.All(skill => Skills.Contains(skill));


    #region Assign Course

    public void AssignCourse(Course course)
    {
        if (AssignedCourses.Contains(course))
            throw new ArgumentException("Course is already assigned");

        if (isOverlappingTime(course))
            throw new ArgumentException("Lesson time is overlapping");

        AssignedCourses.Add(course);
    }

    private bool isOverlappingTime(Course course)
    {
        return AssignedCourses
            .Where(existing => course.Period.OverlapsWith(existing.Period))         // check if date is overlaping
            .Any(existing => course.Schedule
                .Any(newSlot => existing.Schedule
                    .Any(existingSlot => AreTimeSlotsOverlapping(newSlot, existingSlot))));
    }


    private bool AreTimeSlotsOverlapping(TimeSlot slot1, TimeSlot slot2)
    {
        if (slot1.Day != slot2.Day) return false;

        return slot1.Start < slot2.End && slot1.End > slot2.Start;
    }

    #endregion
}