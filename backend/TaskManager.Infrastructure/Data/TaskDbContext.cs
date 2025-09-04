using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Data.Configurations;

namespace TaskManager.Infrastructure.Data;

public class TaskDbContext : DbContext
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
    {
    }

    // DbSets - these represent your tables
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Admin> Admins { get; set; } = null!;
    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<ProjectManager> ProjectManagers { get; set; } = null!;
    public DbSet<TaskItem> Tasks { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new TaskItemConfiguration());

        // Configure User inheritance (Table Per Hierarchy)
        modelBuilder.Entity<User>()
            .HasDiscriminator<string>("UserType")
            .HasValue<Admin>("Admin")
            .HasValue<Client>("Client")
            .HasValue<Employee>("Employee")
            .HasValue<ProjectManager>("ProjectManager");

        // Configure table names
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<TaskItem>().ToTable("Tasks");

        // Configure derived type properties
        modelBuilder.Entity<Admin>()
            .Property(a => a.AdminLevel)
            .HasMaxLength(50);

        modelBuilder.Entity<Client>()
            .Property(c => c.Company)
            .HasMaxLength(100);

        modelBuilder.Entity<Client>()
            .Property(c => c.ContactInfo)
            .HasMaxLength(200);

        modelBuilder.Entity<Employee>()
            .Property(e => e.EmployeeId)
            .HasMaxLength(20);

        modelBuilder.Entity<Employee>()
            .Property(e => e.Department)
            .HasMaxLength(50);

        modelBuilder.Entity<ProjectManager>()
            .Property(pm => pm.ManagerId)
            .HasMaxLength(20);

        modelBuilder.Entity<ProjectManager>()
            .Property(pm => pm.Department)
            .HasMaxLength(50);
    }
}