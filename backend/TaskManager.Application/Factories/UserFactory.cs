using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Factories;

public static class UserFactory
{
    public static User CreateUser(CreateUserDto dto, string hashedPassword)
    {
        User user = dto.Role.ToLower() switch
        {
            "admin" => new Admin 
            { 
                AdminLevel = dto.AdminLevel ?? string.Empty 
            },
            "client" => new Client
            {
                Company = dto.Company ?? string.Empty,
                ContactInfo = dto.ContactInfo ?? string.Empty
            },
            "employee" => new Employee
            {
                EmployeeId = dto.EmployeeId ?? string.Empty,
                Department = dto.Department ?? string.Empty
            },
            "projectmanager" => new ProjectManager
            {
                ManagerId = dto.ManagerId ?? string.Empty,
                Department = dto.Department ?? string.Empty
            },
            _ => throw new Exception($"Invalid role: {dto.Role}. Valid roles: Admin, Client, Employee, ProjectManager")
        };

        // Set common properties
        user.Username = dto.Username;
        user.Email = dto.Email;
        user.PasswordHash = hashedPassword;
        user.CreatedDate = DateTime.UtcNow;

        return user;
    }
}