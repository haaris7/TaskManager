using TaskManager.Domain.Entities;

namespace TaskManager.Application.Interfaces;

public interface ITaskService
    {
    void CreateTask(string title, string description);
    void UpdateTask(int taskId, string title, string description);
    void DeleteTask(int taskId);
    TaskItem GetTaskById(int taskId);
    IEnumerable<TaskItem> GetAllTasks();
    void AssignTask(int taskId, int userId);
    void ChangeTaskStatus(int taskId, string status);
    }
