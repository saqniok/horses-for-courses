using HorsesForCourses.Core;

public static class CourseMapper
{
    public static CourseDto ToDto(Course course)
    {
        return new CourseDto(
            Id: course.Id,
            Title: course.Title,
            StartDate: course.Period.StartDate,
            EndDate: course.Period.EndDate,
            RequiredSkills: course.RequiredSkills.ToList(),
            IsConfirmed: course.IsConfirmed,
            Coach: course.AssignedCoach != null
                ? new CoachShortDto(course.AssignedCoach.Id, course.AssignedCoach.Name) // This line is correct, no change needed here.
                : null
        // Schedule: course.Schedule.Select(ts => ToTimeSlotDto(ts)).ToList()
        );
    }

    public static TimeSlotDto ToTimeSlotDto(TimeSlot ts)
    {
        return new TimeSlotDto
        {
            Day = ts.Day,
            Start = ts.Start,
            End = ts.End
        };
    }


    public static Course ToDomain(CourseDto dto)
    {
        var period = new TimeDay(dto.StartDate, dto.EndDate);
        var course = new Course(dto.Title, period);

        if (dto.RequiredSkills != null)
        {
            course.UpdateRequiredSkills(dto.RequiredSkills);
        }

        if (dto.Schedule != null)
        {
            var timeSlots = dto.Schedule.Select(tsDto => new TimeSlot(tsDto.Day, tsDto.Start, tsDto.End));
            course.UpdateTimeSlot(timeSlots);
        }

        if (dto.IsConfirmed)
        {
            course.Confirm();
        }

        // Если нужно назначить тренера по dto.Coach.Id — делай отдельно, через репозиторий тренеров

        return course;
    }
}
