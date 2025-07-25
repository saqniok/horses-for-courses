using HorsesForCourses.Core;

public static class CourseMapper
{
    public static CourseDto ToDto(Course course)
    {
        return new CourseDto(
            Id: course.Id,
            Title: course.Title,
            PeriodStart: course.Period.StartDate.ToString("yyyy-MM-dd"),
            PeriodEnd: course.Period.EndDate.ToString("yyyy-MM-dd"),
            RequiredSkills: course.RequiredSkills.ToList(),
            IsConfirmed: course.IsConfirmed,
            CoachId: course.AssignedCoach?.Id,
            Schedule: course.Schedule.Select(ts => ToDto(ts)).ToList()
        );
    }

    public static TimeSlotDto ToDto(TimeSlot ts)
    {
        return new TimeSlotDto(
            Day: ts.Day,
            Start: ts.Start,
            End: ts.End
        );
    }
}
