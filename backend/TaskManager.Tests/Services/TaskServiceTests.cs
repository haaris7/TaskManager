using Moq;
using TaskManager.Application.Services;
using TaskManager.Application.Interfaces;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Application.Exceptions;

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

    [Fact]
    public async Task CreateTask_WhenUserDoesNotExist_ShouldThrowException()
    {
        var createTaskDto = new CreateTaskDto
        {
            Name = "Test Task",
            Description = "Test Description",
            StartDate = DateTime.UtcNow,
            AssignedToUserId = 999  // this user does not exist
        };

        // Tell our fake repository to return null (user not found)
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        // Here we expect an exception to be thrown
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            async () => await _taskService.CreateTask(createTaskDto)
        );

        Assert.Equal("User with ID 999 not found", exception.Message);
    }


    // ============ GET TASK TESTS ============

    [Fact]
    public async Task GetTaskById_WhenTaskExists_ShouldReturnTask()
    {
        var testTask = new TaskItem
        {
            Id = 1,
            Name = "Existing Task",
            Description = "Existing Description",
            Status = TaskItemStatus.InProgress,
            AssignedToUserId = 1,
            AssignedTo = new Employee { Username = "jane.doe" },
            StartDate = DateTime.UtcNow,
            CreatedDate = DateTime.UtcNow
        };

        _mockTaskRepository
            .Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(testTask);

        var result = await _taskService.GetTaskById(1);

        Assert.NotNull(result);
        Assert.Equal("Existing Task", result.Name);
        Assert.Equal("InProgress", result.Status);
        Assert.Equal("jane.doe", result.AssignedToUsername);
    }

    [Fact]
    public async Task GetTaskById_WhenTaskDoesNotExist_ShouldReturnNull()
    {
        _mockTaskRepository
            .Setup(repo => repo.GetByIdAsync(999))
            .ReturnsAsync((TaskItem?)null);

        var result = await _taskService.GetTaskById(999);

        Assert.Null(result);
    }

    // ============ DELETE TASK TESTS ============

    [Fact]
    public async Task DeleteTask_WhenTaskExists_ShouldReturnTrue()
    {
    
        var existingTask = new TaskItem { Id = 1, Name = "Task to Delete" };

        _mockTaskRepository
            .Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(existingTask);

        var result = await _taskService.DeleteTask(1);

    
        Assert.True(result);
        
        // Verify that DeleteAsync was called once
        _mockTaskRepository.Verify(repo => repo.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteTask_WhenTaskDoesNotExist_ShouldReturnFalse()
    {
        _mockTaskRepository
            .Setup(repo => repo.GetByIdAsync(999))
            .ReturnsAsync((TaskItem?)null);

        
        var result = await _taskService.DeleteTask(999);

        
        Assert.False(result);
        
        // Verify that DeleteAsync was never called
        _mockTaskRepository.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Never);
    }

    // ============ CHANGE STATUS TESTS ============

    [Fact]
    public async Task ChangeTaskStatus_WithValidStatus_ShouldUpdateStatus()
    {
        
        var existingTask = new TaskItem
        {
            Id = 1,
            Name = "Test Task",
            Status = TaskItemStatus.NotStarted,
            AssignedToUserId = 1,
            AssignedTo = new Employee { Username = "john.doe" }
        };

        _mockTaskRepository
            .Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(existingTask);

        
        var result = await _taskService.ChangeTaskStatus(1, "InProgress");

       
        Assert.NotNull(result);
        Assert.Equal("InProgress", result.Status);
        
        // Verify the repository's UpdateAsync was called
        _mockTaskRepository.Verify(repo => repo.UpdateAsync(It.IsAny<TaskItem>()), Times.Once);
    }

    [Fact]
    public async Task ChangeTaskStatus_WithInvalidStatus_ShouldThrowException()
    {
    
        var existingTask = new TaskItem
        {
            Id = 1,
            Name = "Test Task",
            Status = TaskItemStatus.NotStarted
        };

        _mockTaskRepository
            .Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(existingTask);

      
        var exception = await Assert.ThrowsAsync<ValidationException>(
            async () => await _taskService.ChangeTaskStatus(1, "InvalidStatus")
        );

        Assert.Contains("Invalid status: InvalidStatus", exception.Message);
    }

    // ============ ASSIGN TASK TESTS ============

    [Fact]
    public async Task AssignTask_WhenBothTaskAndUserExist_ShouldReassignSuccessfully()
    {
        // ARRANGE
        var existingTask = new TaskItem
        {
            Id = 1,
            Name = "Task to Reassign",
            AssignedToUserId = 1  // Currently assigned to user 1
        };

        var newUser = new Employee
        {
            Id = 2,
            Username = "new.user"
        };

        _mockTaskRepository
            .Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(existingTask);

        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(2))
            .ReturnsAsync(newUser);

        
        var result = await _taskService.AssignTask(1, 2);

       
        Assert.NotNull(result);
        Assert.Equal(2, result.AssignedToUserId);
        Assert.Equal("new.user", result.AssignedToUsername);
    }

    [Fact]
    public async Task AssignTask_WhenTaskDoesNotExist_ShouldReturnNull()
    {
        
        _mockTaskRepository
            .Setup(repo => repo.GetByIdAsync(999))
            .ReturnsAsync((TaskItem?)null);

      
        var result = await _taskService.AssignTask(999, 1);

       
        Assert.Null(result);
    }



}