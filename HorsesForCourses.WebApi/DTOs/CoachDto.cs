public class CoachDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public List<string> Skills { get; set; } = new();
}

public class UpdateCoachSkillsDto
{
    public List<string> Skills { get; set; } = new();
}



