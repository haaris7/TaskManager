using Moq;
using TaskManager.Application.Services;
using TaskManager.Application.Interfaces;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;

namespace TaskManager.Tests.Services;
// these are Unit tests for TaskService, I might add integration tests later
// I use Moq to mock the repositories
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


    // ============ CREATE TASK TESTS ============

    [Fact]
    public async Task CreateTask_WhenUserExists_ShouldCreateSuccessfully()
    {
        var testUser = new Employee
        {
            Id = 1,
            Username = "haaris.i",
            Email = "haaris@test.com"
        };

        var createTaskDto = new CreateTaskDto
        {
            Name = "Test Task",
            Description = "Test Description",
            StartDate = DateTime.UtcNow,
            AssignedToUserId = 1
        };

        // Ask repository to return the mock user when asked for ID 1
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(testUser);

        // here we run create task from service layer
        var result = await _taskService.CreateTask(createTaskDto);

        Assert.NotNull(result);
        Assert.Equal("Test Task", result.Name);
        Assert.Equal("Test Description", result.Description);
        Assert.Equal("NotStarted", result.Status);
        Assert.Equal("haaris.i", result.AssignedToUsername);
    }

}