using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities;

public class TaskItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public TaskItemStatus Status { get; set; }
    public int AssignedToUserId { get; set; }
    public User AssignedTo { get; set; } = null!;

    public int CreatedByUserId { get; set; }
    public User CreatedBy { get; set; } = null!;
    
    public string Department { get; set; } = string.Empty;
    public string? ClientCompany { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}