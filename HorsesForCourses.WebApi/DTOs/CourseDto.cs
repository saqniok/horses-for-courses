public record CourseDto(
    Guid Id,
    string Title,
    string PeriodStart,
    string PeriodEnd,
    List<string> RequiredSkills,
    List<TimeSlotDto> Schedule,
    bool IsConfirmed,
    Guid? CoachId
);
