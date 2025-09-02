namespace TaskManager.Domain.Entities;

public class Client : User
{
    public string Company { get; set; } = string.Empty;
    public string ContactInfo { get; set; } = string.Empty;
    
    public override string GetDisplayName() => $"{Company} - {Username}";
    public override UserRole GetUserRole() => UserRole.Client;
}