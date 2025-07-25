using System.ComponentModel.DataAnnotations;

public class CoachDto
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public required string Name { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    public List<string> Skills { get; set; } = new();
}

