

namespace HorsesForCourses.Core;


public class Coach
{
    public int Id { get; set; }
    public string Name { get; }
    public string Email { get; }
    public HashSet<string> Skills { get; }

    public List<Course> AssignedCourses { get; }

    public Coach(string name, string email)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Skills = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        AssignedCourses = new List<Course>();
    }

    public void AddSkill(string skill)
    {
        if (Skills.Contains(skill))
            throw new ArgumentException("Skill already added");

        Skills.Add(skill);
    }

    public void RemoveSkill(string skill)
    {
        if (!Skills.Contains(skill))
            throw new ArgumentException("There is no skill in list");

        Skills.Remove(skill);
    }

    public void UpdateSkills(IEnumerable<string> newSkills)
    {
        Skills.Clear();

        foreach (var skill in newSkills)
            AddSkill(skill);
    }

    public bool HasAllSkills(IEnumerable<string> requiredSkills)
        => requiredSkills.All(skill => Skills.Contains(skill));

    #region Assign Course
    public void AssignCourse(Course course)
    {
        EnsureCourseNotAlreadyAssigned(course);
        EnsureNoPeriodOverlap(course);

        AssignedCourses.Add(course);
    }

    private void EnsureCourseNotAlreadyAssigned(Course course)
    {
        if (AssignedCourses.Contains(course))
            throw new ArgumentException("Course is already assigned");
    }

    private void EnsureNoPeriodOverlap(Course course)
    {
        foreach (var existingCourse in AssignedCourses)
        {
            if (!course.Period.OverlapsWith(existingCourse.Period))
                continue;

            EnsureNoScheduleOverlap(course, existingCourse);
        }
    }

    private void EnsureNoScheduleOverlap(Course newCourse, Course existingCourse)
    {
        bool overlapExists = newCourse.Schedule
            .Any(newSlot => existingCourse.Schedule
                .Any(existingSlot => newSlot.OverlapsWith(existingSlot)));

        if (overlapExists)
            throw new ArgumentException("Lesson time is overlapping");
    }
    #endregion

    // AI helped
    public bool IsAvailableCoach()
    {
        if (!AssignedCourses.Any()) return true;

        var allTimeSlots = AssignedCourses.SelectMany(c => c.Schedule).ToList();

        for (int i = 0; i < allTimeSlots.Count; i++)
        {
            for (int j = i + 1; j < allTimeSlots.Count; j++)
            {
                if (AreTimeSlotsOverlapping(allTimeSlots[i], allTimeSlots[j]))
                    throw new ArgumentException("Lesson time is overlapping");
            }
        }

        return true;
    }

    private bool AreTimeSlotsOverlapping(TimeSlot slot1, TimeSlot slot2)
    {
        if (slot1.Day != slot2.Day) return false;

        return slot1.Start < slot2.End && slot1.End > slot2.Start;
    }
}


