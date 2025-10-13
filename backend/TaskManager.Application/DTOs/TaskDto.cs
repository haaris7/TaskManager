namespace TaskManager.Application.DTOs;

/// <summary>
/// DTO for returning task information to the client
/// </summary>
public class TaskDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int AssignedToUserId { get; set; }
    public string AssignedToUsername { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}