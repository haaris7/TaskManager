using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Application.Interfaces;

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
            throw new Exception($"User with ID {createTaskDto.AssignedToUserId} not found");
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

    public void UpdateTask(int taskId, string title, string description)
    {
        throw new NotImplementedException("Will implement later");
    }

    public void DeleteTask(int taskId)
    {
        throw new NotImplementedException("Will implement later");
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


    public void AssignTask(int taskId, int userId)
    {
        throw new NotImplementedException("Will implement later");
    }

    public void ChangeTaskStatus(int taskId, string status)
    {
        throw new NotImplementedException("Will implement later");
    }
}