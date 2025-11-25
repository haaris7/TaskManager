using System.ComponentModel.DataAnnotations;

namespace TaskManager.Application.DTOs;

/// <summary>
/// DTO for updating an existing task
/// </summary>
public class UpdateTaskDto
{
    [Required(ErrorMessage = "Task name is required")]
    [MaxLength(200, ErrorMessage = "Task name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Start date is required")]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Required(ErrorMessage = "Status is required")]
    public string Status { get; set; } = string.Empty;

    [Required(ErrorMessage = "User assignment is required")]
    public int AssignedToUserId { get; set; }


    [Required(ErrorMessage = "Department is required")]
    [MaxLength(50, ErrorMessage = "Department cannot exceed 50 characters")]
    public string Department { get; set; } = string.Empty;

    [MaxLength(100, ErrorMessage = "Client company cannot exceed 100 characters")]
    public string? ClientCompany { get; set; }
}