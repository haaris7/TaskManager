using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Application.DTOs;
using TaskManager.Application.Exceptions;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IUserService userService, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _userService = userService;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> Login(LoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            throw new ValidationException("Invalid email or password");
        }

        return await CreateAuthResponse(user);
    }

    public async Task<AuthResponseDto> Register(RegisterDto registerDto)
    {
        // Validate role-specific required fields
        if (registerDto.AccountType == "Client" && string.IsNullOrWhiteSpace(registerDto.Company))
        {
            throw new ValidationException("Company is required for Client accounts");
        }

        if (registerDto.AccountType == "Employee" && string.IsNullOrWhiteSpace(registerDto.Department))
        {
            throw new ValidationException("Department is required for Employee accounts");
        }

        var createUserDto = new CreateUserDto
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            Password = registerDto.Password,
            Role = registerDto.AccountType,
            // Client fields
            Company = registerDto.Company,
            ContactInfo = registerDto.ContactInfo ?? string.Empty,
            // Employee fields
            Department = registerDto.Department,
            EmployeeId = registerDto.AccountType == "Employee" 
                ? Guid.NewGuid().ToString()[..8] 
                : null
        };

        var userDto = await _userService.CreateUser(createUserDto);
        var user = await _userRepository.GetByIdAsync(userDto.Id);

        return await CreateAuthResponse(user!);
    }

    private async Task<AuthResponseDto> CreateAuthResponse(User user)
    {
        var userDto = await _userService.GetUserById(user.Id);
        var expirationHours = double.Parse(_configuration["JwtSettings:ExpirationHours"] ?? "24");

        return new AuthResponseDto
        {
            Token = GenerateJwtToken(user),
            User = userDto!,
            ExpiresAt = DateTime.UtcNow.AddHours(expirationHours)
        };
    }

    private string GenerateJwtToken(User user)
    {
        var secretKey = _configuration["JwtSettings:SecretKey"] ?? throw new Exception("JWT SecretKey not configured");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.GetUserRole().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(double.Parse(_configuration["JwtSettings:ExpirationHours"] ?? "24")),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}