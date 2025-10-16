namespace TaskManager.Application.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    public string? AdminLevel { get; set; }
    public string? Company { get; set; }
    public string? ContactInfo { get; set; }
    public string? EmployeeId { get; set; }
    public string? ManagerId { get; set; }
    public string? Department { get; set; }
}