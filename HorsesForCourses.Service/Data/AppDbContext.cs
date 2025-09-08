using Microsoft.EntityFrameworkCore;
using HorsesForCourses.Core;
using HorsesForCourses.Service.Data.Mapping;

namespace HorsesForCourses.Service.Data;

public class AppDbContext : DbContext
{
    public DbSet<Coach> Coaches { get; set; } = null!;
    public DbSet<Course> Courses { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new CoachConfiguration());
        modelBuilder.ApplyConfiguration(new CourseConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }

}
