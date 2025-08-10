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
            Title = c.Title
        }).ToList()
    };

    public static Coach ToDomain(CreateCoachDto createDto)
    {
        return new Coach(createDto.Name, createDto.Email);
    }

    public static void UpdateSkills(Coach coach, UpdateCoachSkillsDto dto)
    {
        coach.UpdateSkills(dto.Skills);
    }
}
