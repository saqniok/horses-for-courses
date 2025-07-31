using HorsesForCourses.Core;

public static class CoachMapper
{

    public static CoachSummaryDto ToCoachSummaryDto(Coach coach) => new()
    {
        Id = coach.Id,
        Name = coach.Name,
        Email = coach.Email,
        NumberOfCoursesAssignedTo = coach.AssignedCourses.Count
    };


    public static CoachDetailsDto ToCoachDetailsDto(Coach coach) => new()
    {
        Id = coach.Id,
        Name = coach.Name,
        Email = coach.Email,
        Skills = coach.Skills.ToList(),
        Courses = coach.AssignedCourses.Select(c => new CourseShortDto
        {
            Id = c.Id,
            Name = c.Title
        }).ToList()
    };

    public static Coach ToDomain(CreateCoachDto createDto)
    {
        return new Coach(createDto.Name, createDto.Email);
    }
}
