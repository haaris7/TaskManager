namespace TaskManager.Domain.Entities;

public class Admin : User
{
    public string AdminLevel { get; set; } = string.Empty;
    
    public override string GetDisplayName() => $"Admin: {Username}";
    public override UserRole GetUserRole() => UserRole.Admin;
}