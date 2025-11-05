using System.ComponentModel.DataAnnotations;

namespace TaskManager.Application.DTOs;

public class CreateUserDto
{
    [Required(ErrorMessage = "Username is required")]
    [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "User role is required")]
    public string Role { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? AdminLevel { get; set; }

    [MaxLength(100)]
    public string? Company { get; set; }

    [MaxLength(200)]
    public string? ContactInfo { get; set; }

    [MaxLength(20)]
    public string? EmployeeId { get; set; }

    [MaxLength(20)]
    public string? ManagerId { get; set; }

    [MaxLength(50)]
    public string? Department { get; set; }
}