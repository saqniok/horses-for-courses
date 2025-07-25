using HorsesForCourses.Core;

public static class CoachMapper
{
    public static CoachDto ToDto(Coach coach) => new()
    {
        Id = coach.Id,
        Name = coach.Name,
        Email = coach.Email,
        Skills = coach.Skills.ToList()
    };

    public static Coach ToDomain(CoachDto dto)
    {
        var coach = new Coach(dto.Name, dto.Email);

        foreach (var skill in dto.Skills)
            coach.AddSkill(skill);

        return coach;
    }
}
