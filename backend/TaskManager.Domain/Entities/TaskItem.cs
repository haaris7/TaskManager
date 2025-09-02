namespace TaskManager.Domain.Entities;

public class TaskItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public TaskStatus Status { get; set; }
    public int AssignedToUserId { get; set; }
    public User AssignedTo { get; set; } = null!;

    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}