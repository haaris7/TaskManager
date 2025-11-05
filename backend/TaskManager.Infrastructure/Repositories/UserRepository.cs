using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Data;
using TaskManager.Application.Interfaces;

public class UserRepository : IUserRepository
{
    private readonly TaskDbContext _context;

    public UserRepository(TaskDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> UsernameExistsAsync(string username, int? excludeUserId = null)
    {
        return await _context.Users
            .AnyAsync(u => u.Username.ToLower() == username.ToLower()
                        && (excludeUserId == null || u.Id != excludeUserId));
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null)
    {
        return await _context.Users
            .AnyAsync(u => u.Email.ToLower() == email.ToLower()
                        && (excludeUserId == null || u.Id != excludeUserId));
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(string role)
    {
        // Map role string to discriminator value
        var discriminator = role.ToLower() switch
        {
            "admin" => "Admin",
            "client" => "Client",
            "employee" => "Employee",
            "projectmanager" => "ProjectManager",
            _ => null
        };

        if (discriminator == null)
            return Enumerable.Empty<User>();

        // Use EF.Property to access the discriminator column
        return await _context.Users
            .Where(u => EF.Property<string>(u, "UserType") == discriminator)
            .ToListAsync();
    }
}