public record CreateCourseDto(
    string Title,
    DateTime PeriodStart,
    DateTime PeriodEnd,
    List<string> RequiredSkills,
    List<TimeSlotDto> Schedule
);
public record UpdateSkillsDto(
    List<string> Skills
);

public record UpdateTimeSlotsDto(
    List<TimeSlotDto> TimeSlots
);

public record AssignCoachDto(
    Guid CoachId
);
