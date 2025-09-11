namespace HorsesForCourses.Core;

using System.Globalization;

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
        Name = FormatName(name);
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Skills = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        AssignedCourses = new List<Course>();
    }

    private string FormatName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Coach name cannot be empty.", nameof(name));
            
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.ToLower());
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
        Name = FormatName(name);
        Email = email ?? throw new ArgumentNullException(nameof(email));
    }

    public void RemoveSkill(string skill)
    {
        Skills.Remove(skill);
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

    private bool isOverlappingTime(Course newCourse)
    {
        var newCourseConcreteSlots = newCourse.GetConcreteTimeSlots().ToList();

        foreach (var existingCourse in AssignedCourses)
        {
            // First, check if the course periods overlap at all
            if (!newCourse.Period.OverlapsWith(existingCourse.Period))
            {
                continue;
            }

            var existingCourseConcreteSlots = existingCourse.GetConcreteTimeSlots().ToList();

            foreach (var newSlot in newCourseConcreteSlots)
            {
                foreach (var existingSlot in existingCourseConcreteSlots)
                {
                    if (AreConcreteTimeSlotsOverlapping(newSlot, existingSlot))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool AreConcreteTimeSlotsOverlapping(ConcreteTimeSlot slot1, ConcreteTimeSlot slot2)
    {
        if (slot1.Date != slot2.Date) return false;

        return slot1.Start < slot2.End && slot1.End > slot2.Start;
    }

    #endregion
}
