using System.ComponentModel.DataAnnotations;

public record CourseDto(
    [property: Required] string Title,
    [property: Required] DateTime PeriodStart,
    [property: Required] DateTime PeriodEnd,
    Guid Id = default,
    List<string>? RequiredSkills = null,
    List<TimeSlotDto>? Schedule = null,
    bool IsConfirmed = false,
    Guid? CoachId = null
);

