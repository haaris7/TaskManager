using TaskManager.Application.DTOs;

namespace TaskManager.Application.Interfaces;

public interface ITaskService
{
    Task<TaskDto> CreateTask(CreateTaskDto createTaskDto);
    void UpdateTask(int taskId, string title, string description);
    void DeleteTask(int taskId);
    Task<TaskDto?> GetTaskById(int id);
    Task<IEnumerable<TaskDto>> GetAllTasks();
    void AssignTask(int taskId, int userId);
    void ChangeTaskStatus(int taskId, string status);
}