public record CreateCourseDto(
    string Title,
    DateTime PeriodStart,
    DateTime PeriodEnd,
    List<string> RequiredSkills,
    List<TimeSlotDto> Schedule
);
