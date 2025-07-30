using System.ComponentModel.DataAnnotations;

public record CourseDto(
    int Id,
    [property: Required] string Title,
    [property: Required] DateTime PeriodStart,
    [property: Required] DateTime PeriodEnd,
    List<string>? RequiredSkills = null,
    List<TimeSlotDto>? Schedule = null,
    bool IsConfirmed = false,
    int? CoachId = null
);

public class CourseShortDto
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
}

