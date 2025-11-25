using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Data;
using TaskManager.Application.Interfaces;

public class TaskRepository : ITaskRepository
{
    private readonly TaskDbContext _context;

    public TaskRepository(TaskDbContext context)
    {
        _context = context;
    }

    public async Task<TaskItem?> GetByIdAsync(int id)
    {
        return await _context.Tasks
            .Include(t => t.AssignedTo)
            .Include(t => t.CreatedBy)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync()
    {
            return await _context.Tasks
        .Include(t => t.AssignedTo)
        .Include(t => t.CreatedBy)
        .ToListAsync();
    }

    public async Task AddAsync(TaskItem task)
    {
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TaskItem task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<TaskItem>> GetTasksForUser(int userId, UserRole role, string? department, string? clientCompany)
    {
        IQueryable<TaskItem> query = _context.Tasks
            .Include(t => t.AssignedTo)
            .Include(t => t.CreatedBy);

        return role switch
        {
            UserRole.Admin => await query.ToListAsync(), // Admins see everything
            
            UserRole.ProjectManager => await query
                .Where(t => t.Department == department)
                .ToListAsync(), // PMs see their department only
            
            UserRole.Employee => await query
                .Where(t => t.AssignedToUserId == userId)
                .ToListAsync(), // Employees see only their assigned tasks
            
            UserRole.Client => await query
                .Where(t => t.ClientCompany == clientCompany)
                .ToListAsync(), // Clients see their company's tasks
            
            _ => new List<TaskItem>() // Unknown role = no tasks
        };
    }
}