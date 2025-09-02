namespace TaskManager.Domain.Entities;

public class ProjectManager : User
{
    public string ManagerId { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    
    public override string GetDisplayName() => $"PM {ManagerId}: {Username}";
    public override UserRole GetUserRole() => UserRole.ProjectManager;
}