namespace HorsesForCourses.Service.DTOs;

public class CreateCoachRequest
{
    public required string Name { get; set; }
    public required string Email { get; set; }
}

public class UpdateCoachSkillsDto
{
    public List<string> Skills { get; set; } = new();
}


public class CoachSummaryResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public int NumberOfCoursesAssignedTo { get; set; }
}

public class CoachDetailsDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public List<string> Skills { get; set; } = new();
    public List<CourseShortDto> Courses { get; set; } = new();
}