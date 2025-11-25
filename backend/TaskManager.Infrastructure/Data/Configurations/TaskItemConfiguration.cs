using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Infrastructure.Data.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        // Primary key
        builder.HasKey(t => t.Id);

        // Properties
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.Property(t => t.StartDate)
            .IsRequired();

        builder.Property(t => t.EndDate)
            .IsRequired(false);

        // Convert enum to string in database
        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<string>(); // Stores as "NotStarted", "InProgress", etc.
        
        // NEW: Department property
        builder.Property(t => t.Department)
            .IsRequired()
            .HasMaxLength(50);

        // NEW: Client company (nullable)
        builder.Property(t => t.ClientCompany)
            .HasMaxLength(100);

        builder.Property(t => t.CreatedDate)
            .IsRequired();

        builder.Property(t => t.UpdatedDate)
            .IsRequired(false);

        // Relationships
        builder.HasOne(t => t.AssignedTo)
            .WithMany() // User doesn't have a Tasks navigation property
            .HasForeignKey(t => t.AssignedToUserId)
            .OnDelete(DeleteBehavior.Restrict); // Don't cascade delete
        
        // NEW: Relationship: Task created by a user
        builder.HasOne(t => t.CreatedBy)
            .WithMany()
            .HasForeignKey(t => t.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
            
        // Indexes for better performance
        builder.HasIndex(t => t.AssignedToUserId);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.CreatedDate);
    }
}