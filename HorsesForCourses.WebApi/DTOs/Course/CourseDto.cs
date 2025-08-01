using System.ComponentModel.DataAnnotations;


public record CourseDto(
    int Id,
    [Required] string Title,
    [Required] DateOnly startDate,
    [Required] DateOnly endDate,
    List<string>? RequiredSkills = null,
    List<TimeSlotDto>? Schedule = null,
    bool IsConfirmed = false,
    CoachShortDto? Coach = null
);

public class CourseShortDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
}

public record CreateCourseDto(
    [Required, StringLength(100)]
    string Title,

    [Required]
    DateOnly startDate,

    [Required]
    DateOnly endDate
);


public record UpdateCourseSkillsDto(
    [Required]
    List<string> Skills
);

public record UpdateCourseScheduleDto(
    [Required]
    List<TimeSlotDto> TimeSlots
);

public record AssignCoachDto(
    [Required]
    int CoachId
);

public record CoachShortDto(
    [property: Required] int Id,
    [property: Required] string Name
);

