using System.ComponentModel.DataAnnotations;

namespace TaskManager.Application.DTOs;

public class RegisterDto
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

    [Required(ErrorMessage = "Account type is required")]
    [RegularExpression("^(Employee|Client)$", ErrorMessage = "Account type must be 'Employee' or 'Client'")]
    public string AccountType { get; set; } = string.Empty;

    // Required if AccountType is Client
    [MaxLength(100)]
    public string? Company { get; set; }

    [MaxLength(200)]
    public string? ContactInfo { get; set; }

    // Required if AccountType is Employee
    [MaxLength(50)]
    public string? Department { get; set; }
}