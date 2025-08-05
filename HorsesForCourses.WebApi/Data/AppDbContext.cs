using Microsoft.EntityFrameworkCore;
using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi.Data;

public class AppDbContext : DbContext
{
    public DbSet<Coach> Coaches { get; set; } = null!;
    public DbSet<Course> Courses { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Coach>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(300);

            entity.Property(c => c.Skills)
                .HasConversion(Converters.HashSetToString())
                .Metadata.SetValueComparer(Converters.HashSetComparer());

            entity.HasMany(c => c.AssignedCourses)
                .WithOne(c => c.AssignedCoach)
                .HasForeignKey("AssignedCoachId")  // Shadow property
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.OwnsOne(c => c.Period, pb =>
            {
                pb.Property(p => p.StartDate)
                    .HasColumnName("PeriodStart")
                    .IsRequired();

                pb.Property(p => p.EndDate)
                    .HasColumnName("PeriodEnd")
                    .IsRequired();

            });
            entity.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property<HashSet<string>>("_requiredSkills")
                .HasConversion(Converters.HashSetToString())
                .Metadata.SetValueComparer(Converters.HashSetComparer());

            entity.Ignore(c => c.Schedule);

            entity.Property<int?>("AssignedCoachId").IsRequired(false);
        });
    }
}
