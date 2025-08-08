using HorsesForCourses.Core;
using HorsesForCourses.Tests._tools;
using HorsesForCourses.WebApi.Data;
using Microsoft.EntityFrameworkCore;
using QuickPulse.Show;

namespace HorsesForCourses.Tests.Persistence;

public class CourseCrudTest : CrudTestBase<AppDbContext, Course>
{

    protected override AppDbContext CreateContext(DbContextOptions<AppDbContext> options)
    {
        // new AppDbContext(options).Database.GenerateCreateScript().PulseToLog();
        return new AppDbContext(options);
    }
    private int coachId;
    protected override Task SeedAsync(AppDbContext context)
    {
        var coach = new Coach("Alice", "alice@email.com");
        context.Coaches.Add(coach);
        context.SaveChanges();
        coachId = coach.Id;
        return Task.CompletedTask;
    }


    protected override Course CreateEntity()
    {
        return new Course("my course", new Period(new DateOnly(2025, 8, 1), new DateOnly(2025, 8, 31)));
    }

    protected override Task ModifyEntityAsync(Course entity, AppDbContext context)
    {
        entity.UpdateTimeSlot(new List<TimeSlot> { new TimeSlot(WeekDay.Monday, 10, 12) });
        entity.Confirm();
        var coach = context.Find<Coach>(coachId);
        entity.AssignCoach(coach!);
        return Task.CompletedTask;
    }

    protected override Task AssertUpdatedAsync(Course entity)
    {
        Assert.Equal(WeekDay.Monday, entity.Schedule.First().Day);
        Assert.Equal(10, entity.Schedule.First().Start);
        Assert.Equal(12, entity.Schedule.First().End);
        Assert.NotNull(entity.AssignedCoach);
        return Task.CompletedTask;
    }

    protected override DbSet<Course> GetDbSet(AppDbContext context)
    {
        return context.Courses;
    }

    protected override object[] GetPrimaryKey(Course entity)
    {
        return new object[] { entity.Id };
    }

}