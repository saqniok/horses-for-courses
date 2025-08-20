namespace HorsesForCourses.Blazor.Models;

public record CourseDto(
    int Id,
    string Title,
    DateOnly StartDate,
    DateOnly EndDate
);
