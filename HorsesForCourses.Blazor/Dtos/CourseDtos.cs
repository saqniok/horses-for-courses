using HorsesForCourses.Core;

namespace HorsesForCourses.Blazor.Dtos;

public record CourseDto(
    int Id,
    string Title,
    DateOnly StartDate,
    DateOnly EndDate,
    IReadOnlyList<string>? RequiredSkills = null,
    IReadOnlyList<TimeSlotDto>? Schedule = null,
    bool IsConfirmed = false,
    CoachShortDto? Coach = null
);

public record CoachShortDto(
    int Id,
    string Name
);

public class TimeSlotDto
{
    public WeekDay Day { get; set; }
    public int Start { get; set; }
    public int End { get; set; }
}
