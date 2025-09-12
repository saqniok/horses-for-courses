using System.ComponentModel.DataAnnotations;

namespace HorsesForCourses.MVC.Models.ViewModels;

public class RegisterAccountViewModel
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot be longer than 200 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(300, ErrorMessage = "Email cannot be longer than 300 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
    [DataType(DataType.Password)]
    public string Pass { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password confirmation is required")]
    [DataType(DataType.Password)]
    [Compare("Pass", ErrorMessage = "Passwords do not match")]
    public string ConfirmPass { get; set; } = string.Empty;

    public bool IsCoach { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsUser { get; set; } = true; // Default to User role
}
