using Microsoft.EntityFrameworkCore;
using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Coach> Coaches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Coach entity configuration - minimal for now
        modelBuilder.Entity<Coach>(entity =>
        {
            entity.HasKey(c => c.Id);
            
            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);
                
            entity.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(300);

            // Ignore complex properties for now - we'll add them step by step
            entity.Ignore(c => c.Skills);
            entity.Ignore(c => c.AssignedCourses);
        });
    }
}
