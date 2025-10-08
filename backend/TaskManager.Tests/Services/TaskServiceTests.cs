using Moq;
using TaskManager.Application.Services;
using TaskManager.Application.Interfaces;

namespace TaskManager.Tests.Services;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _mockTaskRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        _mockTaskRepository = new Mock<ITaskRepository>();
        _mockUserRepository = new Mock<IUserRepository>();

        // create the service we want to test using our mock dependencies
        _taskService = new TaskService(_mockTaskRepository.Object, _mockUserRepository.Object);
    }
}