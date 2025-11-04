using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> CreateUser(CreateUserDto createUserDto)
    {
        ValidatePassword(createUserDto.Password);

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);

        var user = new Employee
        {
            Username = createUserDto.Username,
            Email = createUserDto.Email,
            PasswordHash = hashedPassword,
            EmployeeId = createUserDto.EmployeeId ?? string.Empty,
            Department = createUserDto.Department ?? string.Empty,
            CreatedDate = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        return MapToDto(user);
    }

    public async Task<UserDto> UpdateUser(int userId, UpdateUserDto updateUserDto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new Exception($"User with ID {userId} not found");
        }

        user.Username = updateUserDto.Username;
        user.Email = updateUserDto.Email;
        user.UpdatedDate = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        return MapToDto(user);
    }

    public async Task<bool> DeleteUser(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        await _userRepository.DeleteAsync(userId);
        return true;
    }

    public async Task<UserDto?> GetUserById(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        if (user == null)
            return null;

        return MapToDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsers()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToDto);
    }

    public async Task<UserDto?> GetUserByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<UserDto>> GetUsersByRole(string role)
    {
        throw new NotImplementedException();
    }

    private void ValidatePassword(string password)
    {
        var errors = PasswordValidator.Validate(password);
        if (errors.Count != 0)
        {
            throw new Exception(string.Join(". ", errors));
        }
    }

    private UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.GetUserRole().ToString(),
            CreatedDate = user.CreatedDate,
            UpdatedDate = user.UpdatedDate
        };
    }
}