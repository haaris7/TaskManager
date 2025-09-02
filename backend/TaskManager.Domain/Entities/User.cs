namespace TaskManager.Domain.Entities;

public abstract class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    
    public abstract string GetDisplayName();
    public abstract UserRole GetUserRole();
}