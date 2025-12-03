using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    /// <summary>
    /// Create a new task
    /// </summary>
    [Authorize(Policy = "CanCreateTasks")]
    [HttpPost(Name = "CreateTask")]
    public async Task<ActionResult<TaskDto>> CreateTask(CreateTaskDto createTaskDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized("Invalid user token");
        }

        var createdTask = await _taskService.CreateTask(createTaskDto, userId);
        return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, createdTask);
    }

    /// <summary>
    /// Update an existing task by ID
    /// </summary>
    [Authorize(Policy = "CanUpdateAnyTask")]
    [HttpPut("{taskId}", Name = "UpdateTask")]
    public async Task<ActionResult<TaskDto>> UpdateTask(int taskId, UpdateTaskDto updateTaskDto)
    {
        var updatedTask = await _taskService.UpdateTask(taskId, updateTaskDto);
        return Ok(updatedTask);
    }

    /// <summary>
    /// Delete a task by ID
    /// </summary>
    [Authorize(Policy = "CanDeleteTasks")]
    [HttpDelete("{taskId}", Name = "DeleteTask")]
    public async Task<ActionResult> DeleteTask(int taskId)
    {
        var result = await _taskService.DeleteTask(taskId);
        if (!result)
            return NotFound($"Task with ID {taskId} not found");

        return NoContent();
    }

    /// <summary>
    /// Get a single task by ID
    /// </summary>
    [HttpGet("{id}", Name = "GetTaskById")]
    public async Task<ActionResult<TaskDto>> GetTaskById(int id)
    {
        var task = await _taskService.GetTaskById(id);

        if (task == null)
            return NotFound($"Task with ID {id} not found");

        // TODO: Add role-based access check here if needed
        // For now, any authenticated user can view any task by ID

        return Ok(task);
    }

    /// <summary>
    /// Get tasks filtered by the current user's role:
    /// - Admin: All tasks
    /// - ProjectManager: Tasks in their department
    /// - Employee: Tasks assigned to them
    /// - Client: Tasks for their company
    /// </summary>
    [HttpGet(Name = "GetAllTasks")]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetAllTasks()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId) || userRole == null)
        {
            return Unauthorized("Invalid user token");
        }

        var tasks = await _taskService.GetTasksForUser(userId, userRole);
        return Ok(tasks);
    }

    /// <summary>
    /// Assign a task to a user
    /// </summary>
    [Authorize(Policy = "CanAssignTasks")]
    [HttpPost("{taskId}/assign/{userId}", Name = "AssignTask")]
    public async Task<ActionResult<TaskDto>> AssignTask(int taskId, int userId)
    {
        var updatedTask = await _taskService.AssignTask(taskId, userId);
        if (updatedTask == null)
            return NotFound($"Task with ID {taskId} or User with ID {userId} not found");

        return Ok(updatedTask);
    }

    /// <summary>
    /// Change the status of a task
    /// Admin/PM can change any task status (within their scope)
    /// Employee can only change status of tasks assigned to them
    /// </summary>
    [Authorize(Policy = "CanChangeTaskStatus")]
    [HttpPost("{taskId}/status/{status}", Name = "ChangeTaskStatus")]
    public async Task<ActionResult<TaskDto>> ChangeTaskStatus(int taskId, string status)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized("Invalid user token");
        }

        // If user is Employee, verify they own the task
        if (userRole == "Employee")
        {
            var task = await _taskService.GetTaskById(taskId);
            if (task == null)
                return NotFound($"Task with ID {taskId} not found");
            
            if (task.AssignedToUserId != userId)
                return Forbid("Employees can only change status of their own assigned tasks");
        }

        var updatedTask = await _taskService.ChangeTaskStatus(taskId, status);
        if (updatedTask == null)
            return NotFound($"Task with ID {taskId} not found or invalid status '{status}'");

        return Ok(updatedTask);
    }
}