using HorsesForCourses.Core;

public static class CoachMapper
{
    // public static CoachDto ToDto(Coach coach) => new()
    // {
    //     Id = coach.Id,
    //     Name = coach.Name,
    //     Email = coach.Email,
    //     Skills = coach.Skills.ToList(),
    //     AssignedCourses = coach.AssignedCourses.Select(c => new CourseShortDto
    //     {
    //         Id = c.Id,
    //         Title = c.Title
    //     }).ToList()
    // };

    // public static Coach ToDomain(CoachDto dto)
    // {
    //     var coach = new Coach(dto.Name, dto.Email);

    //     foreach (var skill in dto.Skills)
    //         coach.AddSkill(skill);

    //     return coach;
    // }

    public static CoachSummaryDto ToCoachSummaryDto(Coach coach) => new()
    {
        Id = coach.Id,
        Name = coach.Name,
        Email = coach.Email,
        NumberOfCoursesAssignedTo = coach.AssignedCourses.Count
    };

    // Маппинг для GET /coaches/{id}
    // Преобразует Coach в CoachDetailsDto
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
    
    // Маппинг для POST /coaches
    // Преобразует CreateCoachDto в Coach
    public static Coach ToDomain(CreateCoachDto createDto)
    {
        return new Coach(createDto.Name, createDto.Email);
    }
}
