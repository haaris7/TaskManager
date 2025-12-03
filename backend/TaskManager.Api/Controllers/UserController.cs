using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize(Policy = "CanManageUsers")]
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createUserDto)
    {
        var createdUser = await _userService.CreateUser(createUserDto);
        return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
    }

    // Changed from CanManageUsers to CanViewUsers - PMs need this for task assignment
    [Authorize(Policy = "CanViewUsers")]
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        var user = await _userService.GetUserById(id);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    // Changed from CanManageUsers to CanViewUsers - PMs need this for task assignment dropdowns
    [Authorize(Policy = "CanViewUsers")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsers();
        return Ok(users);
    }

    // Changed from CanManageUsers to CanViewUsers
    [Authorize(Policy = "CanViewUsers")]
    [HttpGet("email/{email}")]
    public async Task<ActionResult<UserDto>> GetUserByEmail(string email)
    {
        var user = await _userService.GetUserByEmail(email);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    // Changed from CanManageUsers to CanViewUsers
    [Authorize(Policy = "CanViewUsers")]
    [HttpGet("role/{role}")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersByRole(string role)
    {
        var users = await _userService.GetUsersByRole(role);
        return Ok(users);
    }

    [Authorize(Policy = "CanManageUsers")]
    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> UpdateUser(int id, UpdateUserDto updateUserDto)
    {
        var updatedUser = await _userService.UpdateUser(id, updateUserDto);
        return Ok(updatedUser);
    }

    [Authorize(Policy = "CanManageUsers")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        var result = await _userService.DeleteUser(id);

        if (!result)
            return NotFound();

        return NoContent();
    }
}