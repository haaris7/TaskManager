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

    public TaskService(ITaskRepository taskRepository, IUserRepository userRepository)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
    }

    public async Task<TaskDto> CreateTask(CreateTaskDto createTaskDto, int createdByUserId)
    {
        var user = await _userRepository.GetByIdAsync(createTaskDto.AssignedToUserId);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {createTaskDto.AssignedToUserId} not found");
        }

        var creator = await _userRepository.GetByIdAsync(createdByUserId);
        if (creator == null)
        {
            throw new NotFoundException($"Creator with ID {createdByUserId} not found");
        }

        var taskItem = new TaskItem
        {
            Name = createTaskDto.Name,
            Description = createTaskDto.Description,
            StartDate = createTaskDto.StartDate,
            EndDate = createTaskDto.EndDate,
            Status = TaskItemStatus.NotStarted,
            AssignedToUserId = createTaskDto.AssignedToUserId,
            CreatedByUserId = createdByUserId,
            Department = createTaskDto.Department,
            ClientCompany = createTaskDto.ClientCompany,
            CreatedDate = DateTime.UtcNow
        };

        await _taskRepository.AddAsync(taskItem);

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
            CreatedByUserId = createdByUserId,
            CreatedByUsername = creator.Username,
            Department = taskItem.Department,
            ClientCompany = taskItem.ClientCompany,
            CreatedDate = taskItem.CreatedDate,
            UpdatedDate = taskItem.UpdatedDate
        };
    }

    public async Task<TaskDto> UpdateTask(int taskId, UpdateTaskDto updateTaskDto)
    {
        var task = await _taskRepository.GetByIdAsync(taskId) 
            ?? throw new NotFoundException($"Task with ID {taskId} not found");

        var user = await _userRepository.GetByIdAsync(updateTaskDto.AssignedToUserId) 
            ?? throw new NotFoundException($"User with ID {updateTaskDto.AssignedToUserId} not found");

        task.Name = updateTaskDto.Name;
        task.Description = updateTaskDto.Description;
        task.StartDate = updateTaskDto.StartDate;
        task.EndDate = updateTaskDto.EndDate;
        task.AssignedToUserId = updateTaskDto.AssignedToUserId;
        task.Department = updateTaskDto.Department;
        task.ClientCompany = updateTaskDto.ClientCompany;
        task.UpdatedDate = DateTime.UtcNow;

        if (Enum.TryParse<TaskItemStatus>(updateTaskDto.Status, out var status))
        {
            task.Status = status;
        }
        else
        {
            throw new ValidationException($"Invalid status: {updateTaskDto.Status}");
        }

        await _taskRepository.UpdateAsync(task);

        return MapToDto(task);
    }

    public async Task<bool> DeleteTask(int taskId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
        {
            return false;
        }

        await _taskRepository.DeleteAsync(taskId);
        return true;
    }

    public async Task<TaskDto?> GetTaskById(int id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        return task == null ? null : MapToDto(task);
    }

    public async Task<IEnumerable<TaskDto>> GetAllTasks()
    {
        var tasks = await _taskRepository.GetAllAsync();
        return tasks.Select(MapToDto);
    }

    public async Task<IEnumerable<TaskDto>> GetTasksForUser(int userId, string role)
    {
        // Parse the role string to enum
        if (!Enum.TryParse<UserRole>(role, out var userRole))
        {
            throw new ValidationException($"Invalid role: {role}");
        }

        // Get user details for department/company filtering
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {userId} not found");
        }

        // Extract department or company based on user type
        string? department = null;
        string? clientCompany = null;

        switch (user)
        {
            case ProjectManager pm:
                department = pm.Department;
                break;
            case Employee emp:
                department = emp.Department;
                break;
            case Client client:
                clientCompany = client.Company;
                break;
        }

        var tasks = await _taskRepository.GetTasksForUser(userId, userRole, department, clientCompany);
        return tasks.Select(MapToDto);
    }

    public async Task<TaskDto?> AssignTask(int taskId, int userId)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
        {
            return null;
        }

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {userId} not found");
        }

        task.AssignedToUserId = userId;
        task.UpdatedDate = DateTime.UtcNow;

        await _taskRepository.UpdateAsync(task);

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
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
        {
            return null;
        }

        if (Enum.TryParse<TaskItemStatus>(status, out var taskStatus))
        {
            task.Status = taskStatus;
            task.UpdatedDate = DateTime.UtcNow;

            await _taskRepository.UpdateAsync(task);

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

    private TaskDto MapToDto(TaskItem task)
    {
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
            CreatedByUserId = task.CreatedByUserId,
            CreatedByUsername = task.CreatedBy?.Username ?? "Unknown",
            Department = task.Department,
            ClientCompany = task.ClientCompany,
            CreatedDate = task.CreatedDate,
            UpdatedDate = task.UpdatedDate
        };
    }
}