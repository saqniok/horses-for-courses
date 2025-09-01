using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HorsesForCourses.Core;
using HorsesForCourses.Service.Data;

namespace HorsesForCourses.Service.Data.Mapping
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> entity)
        {
            entity.ToTable("Courses");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.OwnsOne(c => c.Period, pb =>
            {
                pb.Property(p => p.StartDate)
                    .HasColumnName("PeriodStart")
                    .IsRequired();

                pb.Property(p => p.EndDate)
                    .HasColumnName("PeriodEnd")
                    .IsRequired();
            });

            entity.Property("_requiredSkills")
                  .HasColumnName("RequiredSkills")
                  .HasConversion(Converters.HashSetToString())
                  .Metadata.SetValueComparer(Converters.HashSetComparer());

            entity.OwnsMany(c => c.Schedule, ts =>
            {
                ts.ToTable("CourseSchedule");

                ts.HasKey("Id");
                ts.WithOwner().HasForeignKey("CourseId");

                ts.Property(t => t.Day)
                    .HasConversion(v => v.ToString(), v => Enum.Parse<WeekDay>(v))
                    .HasColumnName("Day")
                    .IsRequired();

                ts.Property(t => t.Start)
                    .HasColumnName("Start")
                    .IsRequired();

                ts.Property(t => t.End)
                    .HasColumnName("End")
                    .IsRequired();
            });

            entity.HasOne(c => c.AssignedCoach)
                .WithMany(c => c.AssignedCourses)
                .HasForeignKey("CoachId")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
