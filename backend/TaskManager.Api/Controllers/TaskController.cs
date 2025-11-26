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
        try
        {
            var updatedTask = await _taskService.UpdateTask(taskId, updateTaskDto);
            return Ok(updatedTask);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Delete a task by ID
    /// </summary>
    [Authorize(Policy = "CanDeleteTasks")]
    [HttpDelete("{taskId}", Name = "DeleteTask")]
    public async Task<ActionResult> DeleteTask(int taskId)
    {
        try
        {
            var result = await _taskService.DeleteTask(taskId);
            if (!result)
                return NotFound($"Task with ID {taskId} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
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

        return Ok(task);
    }

    /// <summary>
    /// Get all tasks
    /// </summary>
    [HttpGet(Name = "GetAllTasks")]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetAllTasks()
    {
        var tasks = await _taskService.GetAllTasks();
        return Ok(tasks);
    }

    /// <summary>
    /// Assign a task to a user
    ///     </summary>
    [Authorize(Policy = "CanAssignTasks")]
    [HttpPost("{taskId}/assign/{userId}", Name = "AssignTask")]
    public async Task<ActionResult<TaskDto>> AssignTask(int taskId, int userId)
    {
        try
        {
            var updatedTask = await _taskService.AssignTask(taskId, userId);
            if (updatedTask == null)
                return NotFound($"Task with ID {taskId} or User with ID {userId} not found");

            return Ok(updatedTask);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Change the status of a task
    ///    </summary>
    /// <param name="taskId">ID of the task to change status</param>
    /// <param name="status">New status value (e.g., "Pending", "
    [HttpPost("{taskId}/status/{status}", Name = "ChangeTaskStatus")]
    public async Task<ActionResult<TaskDto>> ChangeTaskStatus(int taskId, string status)
    {
        try
        {
            var updatedTask = await _taskService.ChangeTaskStatus(taskId, status);
            if (updatedTask == null)
                return NotFound($"Task with ID {taskId} not found or invalid status '{status}'");

            return Ok(updatedTask);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }





}