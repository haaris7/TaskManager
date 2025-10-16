using TaskManager.Application.DTOs;

namespace TaskManager.Application.Interfaces;

public interface IUserService
{
    Task<UserDto> CreateUser(CreateUserDto createUserDto);
    Task<UserDto> UpdateUser(int userId, UpdateUserDto updateUserDto);
    Task<bool> DeleteUser(int userId);
    Task<UserDto?> GetUserById(int id);
    Task<IEnumerable<UserDto>> GetAllUsers();
    Task<UserDto?> GetUserByEmail(string email);
    Task<IEnumerable<UserDto>> GetUsersByRole(string role);
}