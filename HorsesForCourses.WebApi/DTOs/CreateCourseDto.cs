using System.ComponentModel.DataAnnotations;

public record CreateCourseDto(
    [Required, StringLength(100)]
    string Title,

    [Required]
    DateTime PeriodStart,

    [Required]
    DateTime PeriodEnd
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