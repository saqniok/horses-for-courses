using HorsesForCourses.Core;
using System.ComponentModel.DataAnnotations;

namespace HorsesForCourses.Blazor.Dtos;

public class CourseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public IReadOnlyList<string>? RequiredSkills { get; set; }
    public IReadOnlyList<TimeSlotDto>? Schedule { get; set; }
    public bool IsConfirmed { get; set; }
    public CoachShortDto? Coach { get; set; }
}

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

// Added CreateCourseRequestDto
public class CreateCourseRequestDto : IValidatableObject
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters.")]
    public required string Title { get; set; }

    [Required(ErrorMessage = "Start Date is required.")]
    public DateOnly StartDate { get; set; }

    [Required(ErrorMessage = "End Date is required.")]
    public DateOnly EndDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate > EndDate)
        {
            yield return new ValidationResult(
                "Start Date cannot be after End Date."); // Corrected line
        }
    }
}