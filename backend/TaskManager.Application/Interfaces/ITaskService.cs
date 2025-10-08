using TaskManager.Application.DTOs;

namespace TaskManager.Application.Interfaces;

public interface ITaskService
{
    Task<TaskDto> CreateTask(CreateTaskDto createTaskDto);
    Task<TaskDto> UpdateTask(int taskId, UpdateTaskDto updateTaskDto);
    Task<bool> DeleteTask(int taskId);
    Task<TaskDto?> GetTaskById(int id);
    Task<IEnumerable<TaskDto>> GetAllTasks();
    Task<TaskDto?> AssignTask(int taskId, int userId);
    Task<TaskDto?> ChangeTaskStatus(int taskId, string status);
}