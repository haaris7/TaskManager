using TaskManager.Application.DTOs;

namespace TaskManager.Application.Interfaces{

    public interface IAuthService
    {
        Task<AuthResponseDto> Login(LoginDto loginDto);
        Task<AuthResponseDto> Register(CreateUserDto createUserDto);
    }
}