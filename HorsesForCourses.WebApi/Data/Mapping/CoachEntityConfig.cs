using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HorsesForCourses.Core.EntityConfigurations
{
    public class CoachConfiguration : IEntityTypeConfiguration<Coach>
    {
        public void Configure(EntityTypeBuilder<Coach> entity)
        {
            entity.ToTable("Coaches");

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
                .WithOne(course => course.AssignedCoach)
                .HasForeignKey("AssignedCoachId") // Shadow property
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
