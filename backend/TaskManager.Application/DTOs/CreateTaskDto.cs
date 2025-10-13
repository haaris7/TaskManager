using System.ComponentModel.DataAnnotations;

namespace TaskManager.Application.DTOs;

/// <summary>
/// DTO for creating a new task
/// </summary>
public class CreateTaskDto
{
    [Required(ErrorMessage = "Task name is required")]
    [MaxLength(200, ErrorMessage = "Task name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Start date is required")]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Required(ErrorMessage = "User assignment is required")]
    public int AssignedToUserId { get; set; }
}