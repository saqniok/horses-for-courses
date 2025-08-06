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

/**
    Это объекты передачи данных (DTO) для сущности Course. DTO используются для обмена данными между API и внешними клиентами. Они могут отличаться от доменных моделей, чтобы:

    - Скрыть внутреннюю структуру: Не все поля доменной модели могут быть нужны клиенту, или их названия могут быть изменены для удобства API.
    - Предоставить данные в удобном формате: Например, DateOnly может быть представлен как строка.
    - Обеспечить валидацию для API: Атрибуты [Required] и [StringLength] используются для валидации входных данных на уровне API.
    В этом файле определены несколько DTO:

    - CourseDto: Используется для получения полной информации о курсе через API. Включает Id, Title, startDate, endDate, RequiredSkills, Schedule, IsConfirmed и Coach.
    - CourseShortDto: Упрощенная версия CourseDto, вероятно, для краткого представления курса (например, в списках).
    - CreateCourseDto: Используется для создания нового курса. Содержит только Title, startDate и endDate, так как остальные поля могут быть установлены сервером.
    - UpdateCourseSkillsDto: Используется для обновления навыков курса.
    - UpdateCourseScheduleDto: Используется для обновления расписания курса.
    - AssignCoachDto: Используется для назначения тренера курсу.
    - CoachShortDto: Упрощенная версия DTO для тренера, используемая внутри CourseDto.
*/