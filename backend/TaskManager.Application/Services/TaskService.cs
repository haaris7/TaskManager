using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Exceptions;


namespace TaskManager.Application.Services;


public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;

    // Constructor - dependencies are injected here
    public TaskService(ITaskRepository taskRepository, IUserRepository userRepository)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
    }

    public async Task<TaskDto> CreateTask(CreateTaskDto createTaskDto)
    {
        // Check if the assigned user exists
        var user = await _userRepository.GetByIdAsync(createTaskDto.AssignedToUserId);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {createTaskDto.AssignedToUserId} not found");
        }

        // Create a new TaskItem entity from the DTO
        var taskItem = new TaskItem
        {
            Name = createTaskDto.Name,
            Description = createTaskDto.Description,
            StartDate = createTaskDto.StartDate,
            EndDate = createTaskDto.EndDate,
            Status = TaskItemStatus.NotStarted,
            AssignedToUserId = createTaskDto.AssignedToUserId,
            CreatedDate = DateTime.UtcNow
        };

        // Save to database
        await _taskRepository.AddAsync(taskItem);

        // Return the created task as a DTO
        return new TaskDto
        {
            Id = taskItem.Id,
            Name = taskItem.Name,
            Description = taskItem.Description,
            StartDate = taskItem.StartDate,
            EndDate = taskItem.EndDate,
            Status = taskItem.Status.ToString(),
            AssignedToUserId = taskItem.AssignedToUserId,
            AssignedToUsername = user.Username,
            CreatedDate = taskItem.CreatedDate,
            UpdatedDate = taskItem.UpdatedDate
        };
    }

      public async Task<TaskDto> UpdateTask(int taskId, UpdateTaskDto updateTaskDto)
    {
        // Get the existing task
        var task = await _taskRepository.GetByIdAsync(taskId) ?? throw new NotFoundException($"Task with ID {taskId} not found");

        // Check if the assigned user exists
        var user = await _userRepository.GetByIdAsync(updateTaskDto.AssignedToUserId) ?? throw new NotFoundException($"User with ID {updateTaskDto.AssignedToUserId} not found");

        // Update the task properties
        task.Name = updateTaskDto.Name;
        task.Description = updateTaskDto.Description;
        task.StartDate = updateTaskDto.StartDate;
        task.EndDate = updateTaskDto.EndDate;
        task.AssignedToUserId = updateTaskDto.AssignedToUserId;
        task.UpdatedDate = DateTime.UtcNow;

        // Parse and set status
        if (Enum.TryParse<TaskItemStatus>(updateTaskDto.Status, out var status))
        {
            task.Status = status;
        }
        else
        {
            throw new ValidationException($"Invalid status: {updateTaskDto.Status}");
        }

        // Save changes
        await _taskRepository.UpdateAsync(task);

        // Return updated task as DTO
        return new TaskDto
        {
            Id = task.Id,
            Name = task.Name,
            Description = task.Description,
            StartDate = task.StartDate,
            EndDate = task.EndDate,
            Status = task.Status.ToString(),
            AssignedToUserId = task.AssignedToUserId,
            AssignedToUsername = user.Username,
            CreatedDate = task.CreatedDate,
            UpdatedDate = task.UpdatedDate
        };
    }

    public async Task<bool> DeleteTask(int taskId)
    {
        // Check if task exists
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
        {
            return false;
        }

        // Delete the task
        await _taskRepository.DeleteAsync(taskId);
        return true;
    }

    public async Task<TaskDto?> GetTaskById(int id)
    {
        // Get task from repository
        var task = await _taskRepository.GetByIdAsync(id);

        // If task doesn't exist, return null
        if (task == null)
            return null;

        // Convert TaskItem to TaskDto
        return new TaskDto
        {
            Id = task.Id,
            Name = task.Name,
            Description = task.Description,
            StartDate = task.StartDate,
            EndDate = task.EndDate,
            Status = task.Status.ToString(),
            AssignedToUserId = task.AssignedToUserId,
            AssignedToUsername = task.AssignedTo?.Username ?? "Unknown",
            CreatedDate = task.CreatedDate,
            UpdatedDate = task.UpdatedDate
        };
    }

    public async Task<IEnumerable<TaskDto>> GetAllTasks()
    {
        // Get all tasks from repository
        var tasks = await _taskRepository.GetAllAsync();

        // Convert each TaskItem to TaskDto
        var taskDtos = new List<TaskDto>();
        foreach (var task in tasks)
        {
            taskDtos.Add(new TaskDto
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                StartDate = task.StartDate,
                EndDate = task.EndDate,
                Status = task.Status.ToString(),
                AssignedToUserId = task.AssignedToUserId,
                AssignedToUsername = task.AssignedTo?.Username ?? "Unknown",
                CreatedDate = task.CreatedDate,
                UpdatedDate = task.UpdatedDate
            });
        }

        return taskDtos;
    }


    public async Task<TaskDto?> AssignTask(int taskId, int userId)
    {
        // Get the task
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
        {
            return null;
        }

        // Check if user exists
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {userId} not found");
        }

        // Reassign the task
        task.AssignedToUserId = userId;
        task.UpdatedDate = DateTime.UtcNow;

        // Save changes
        await _taskRepository.UpdateAsync(task);

        // Return updated task
        return new TaskDto
        {
            Id = task.Id,
            Name = task.Name,
            Description = task.Description,
            StartDate = task.StartDate,
            EndDate = task.EndDate,
            Status = task.Status.ToString(),
            AssignedToUserId = task.AssignedToUserId,
            AssignedToUsername = user.Username,
            CreatedDate = task.CreatedDate,
            UpdatedDate = task.UpdatedDate
        };
    }

    public async Task<TaskDto?> ChangeTaskStatus(int taskId, string status)
    {
        // Get the task
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
        {
            return null;
        }

        // Parse and validate status
        if (Enum.TryParse<TaskItemStatus>(status, out var taskStatus))
        {
            task.Status = taskStatus;
            task.UpdatedDate = DateTime.UtcNow;

            // Save changes
            await _taskRepository.UpdateAsync(task);

            // Return updated task
            return new TaskDto
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                StartDate = task.StartDate,
                EndDate = task.EndDate,
                Status = task.Status.ToString(),
                AssignedToUserId = task.AssignedToUserId,
                AssignedToUsername = task.AssignedTo?.Username ?? "Unknown",
                CreatedDate = task.CreatedDate,
                UpdatedDate = task.UpdatedDate
            };
        }
        else
        {
            throw new ValidationException($"Invalid status: {status}. Valid values: NotStarted, InProgress, Completed, OnHold, Cancelled");
        }
    }
}