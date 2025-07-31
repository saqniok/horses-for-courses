using HorsesForCourses.Core;

public static class CourseMapper
{
    public static CourseDto ToDto(Course course)
    {
        return new CourseDto(
            Id: course.Id,
            Title: course.Title,
            startDate: course.Period.StartDate.ToString("yyyy-MM-dd"),
            endDate: course.Period.EndDate.ToString("yyyy-MM-dd"),
            RequiredSkills: course.RequiredSkills.ToList(),
            IsConfirmed: course.IsConfirmed,
            Coach: course.AssignedCoach != null ? new CoachShortDto(course.AssignedCoach.Id, course.AssignedCoach.Name) : null,
            Schedule: course.Schedule.Select(ts => ToTimeSlotDto(ts)).ToList()
        );
    }

    public static TimeSlotDto ToTimeSlotDto(TimeSlot ts)
    {
        return new TimeSlotDto(
            Day: ts.Day,
            Start: ts.Start,
            End: ts.End
        );
    }

    public static Course ToDomain(CourseDto dto)
    {
        var period = new Period(DateTime.Parse(dto.startDate), DateTime.Parse(dto.endDate));

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

        //TODO:
        // Назначение тренера по CoachId можно сделать отдельно, если есть доступ к репозиторию тренеров

        return course;
    }
}
