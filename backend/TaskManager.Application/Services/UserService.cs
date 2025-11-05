using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Application.Factories;

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
        // Validate password
        ValidatePassword(createUserDto.Password);
        
        // Validate uniqueness
        await ValidateUniqueUsername(createUserDto.Username);
        await ValidateUniqueEmail(createUserDto.Email);

        // Hash password
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
        
        // Create user using factory
        var user = UserFactory.CreateUser(createUserDto, hashedPassword);

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

        // Validate that the username and email are unique before updating
        await ValidateUniqueUsername(updateUserDto.Username, userId);
        await ValidateUniqueEmail(updateUserDto.Email, userId);

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

    private async Task ValidateUniqueUsername(string username, int? excludeUserId = null)
    {
        if (await _userRepository.UsernameExistsAsync(username, excludeUserId))
        {
            throw new Exception($"Username '{username}' is already taken");
        }
    }

    private async Task ValidateUniqueEmail(string email, int? excludeUserId = null)
    {
        if (await _userRepository.EmailExistsAsync(email, excludeUserId))
        {
            throw new Exception($"Email '{email}' is already registered");
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