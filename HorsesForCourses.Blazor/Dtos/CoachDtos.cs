using System.ComponentModel.DataAnnotations;

namespace HorsesForCourses.Blazor.Dtos;

public class CoachSummaryResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public int NumberOfCoursesAssignedTo { get; set; }
}

public class CreateCoachRequest
{
    [Required(ErrorMessage = "Name is required.")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public required string Email { get; set; }
}

public class UpdateCoachSkillsDto
{
    public List<string> Skills { get; set; } = new();
}

public class CoachDetailsDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public required string Email { get; set; }
    public List<string> Skills { get; set; } = new();
    public List<CourseShortDto> Courses { get; set; } = new();
}

public record CourseShortDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
}
