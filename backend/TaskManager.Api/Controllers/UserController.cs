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
        try
        {
            var createdUser = await _userService.CreateUser(createUserDto);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Policy = "CanManageUsers")]
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        var user = await _userService.GetUserById(id);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [Authorize(Policy = "CanManageUsers")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsers();
        return Ok(users);
    }

    [Authorize(Policy = "CanManageUsers")]
    [HttpGet("email/{email}")]
    public async Task<ActionResult<UserDto>> GetUserByEmail(string email)
    {
        var user = await _userService.GetUserByEmail(email);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [Authorize(Policy = "CanManageUsers")]
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
        try
        {
            var updatedUser = await _userService.UpdateUser(id, updateUserDto);
            return Ok(updatedUser);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
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