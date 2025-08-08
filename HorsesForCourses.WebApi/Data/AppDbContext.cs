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

            // entity.HasMany(c => c.AssignedCourses)
            //     .WithOne(c => c.AssignedCoach)
            //     .HasForeignKey("AssignedCoachId")  // Shadow property
            //     .OnDelete(DeleteBehavior.Cascade); 

                
        });


        // Course
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

            entity.OwnsMany(c => c.Schedule, ts =>
            {
                ts.HasKey("Id");
                ts.WithOwner().HasForeignKey("CourseId");

                ts.Property(t => t.Day)
                    .HasConversion(
                        v => v.ToString(),
                        v => Enum.Parse<WeekDay>(v))
                    .HasColumnName("Day");

                ts.Property(t => t.Start).HasColumnName("Start");
                ts.Property(t => t.End).HasColumnName("End");

                // ts.HasKey("BookingId", "Day", "Start", "End");
                // ts.ToTable("BookingTimeslots");
            });

            // ts.WithOwner().HasForeignKey("BookingId");

            //         ts.Property(t => t.Day)
            //             .HasConversion(
            //                 v => v.ToString(),
            //                 v => Enum.Parse<DayOfWeek>(v))
            //             .HasColumnName("Day");

            //         ts.Property(t => t.Start).HasColumnName("Start");
            //         ts.Property(t => t.End).HasColumnName("End");

            //         ts.HasKey("BookingId", "Day", "Start", "End");
            //         ts.ToTable("BookingTimeslots");

            entity.HasOne(c => c.AssignedCoach)
                .WithMany(c => c.AssignedCourses)
                .HasForeignKey("CoachId")
                .IsRequired(false);
        });
    }
}
