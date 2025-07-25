using HorsesForCourses.Core;

public static class CourseMapper
{
    public static CourseDto ToDto(Course course)
    {
        return new CourseDto(
            Id: course.Id,
            Title: course.Title,
            PeriodStart: course.Period.StartDate,
            PeriodEnd: course.Period.EndDate,
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

    public static Course ToDomain(CourseDto dto)
    {
        var period = new Period(dto.PeriodStart, dto.PeriodEnd);

        var course = new Course(dto.Title, period);

        if (dto.RequiredSkills != null)
        {
            foreach (var skill in dto.RequiredSkills)
                course.AddRequiredSkill(skill);
        }

        if (dto.Schedule != null)
        {
            foreach (var tsDto in dto.Schedule)
            {
                var timeSlot = new TimeSlot(tsDto.Day, tsDto.Start, tsDto.End);
                course.AddTimeSlot(timeSlot);
            }
        }

        if (dto.IsConfirmed)
        {
            course.Confirm();
        }

        // Назначение тренера по CoachId можно сделать отдельно, если есть доступ к репозиторию тренеров

        return course;
    }
}
