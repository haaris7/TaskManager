namespace TaskManager.Domain.Entities;

public class Employee : User
{
    public string EmployeeId { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    
    public override string GetDisplayName() => $"{EmployeeId}: {Username}";
    public override UserRole GetUserRole() => UserRole.Employee;
}