using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi.DTOs;

public static class CourseMapper
{
    // public static CourseDto ToDto(Course course)
    // {
    //     return new CourseDto(
    //         Id: course.Id,
    //         Title: course.Title,
    //         StartDate: course.Period.StartDate,
    //         EndDate: course.Period.EndDate,
    //         RequiredSkills: course.RequiredSkills.ToList(),
    //         IsConfirmed: course.IsConfirmed,
    //         Coach: course.AssignedCoach != null
    //             ? new CoachShortDto(course.AssignedCoach.Id, course.AssignedCoach.Name) // This line is correct, no change needed here.
    //             : null
    //     );
    // }

    // public static TimeSlotDto ToTimeSlotDto(TimeSlot ts)
    // {
    //     return new TimeSlotDto
    //     {
    //         Day = ts.Day,
    //         Start = ts.Start,
    //         End = ts.End
    //     };
    // }


    public static Course ToDomain(CreateCourseRequest dto)
    {
        var period = new TimeDay(dto.startDate, dto.endDate);
        var course = new Course(dto.Title, period);
        return course;
    }

}
