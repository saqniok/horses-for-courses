using HorsesForCourses.Core;
using HorsesForCourses.WebApi.Data;
using Microsoft.EntityFrameworkCore;


public class EFCourseRepositoryTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldAddCourse()
    {
        var context = GetInMemoryDbContext();
        var repository = new EFCourseRepository(context);

        var course = new Course(
            "Poker Basics",
            new TimeDay(DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(5)))
        );

        await repository.AddAsync(course);
        await repository.SaveChangesAsync();

        var allCourses = await repository.GetAllAsync();

        Assert.Single(allCourses); 
        Assert.Equal("Poker Basics", allCourses.First().Title); 
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectCourse()
    {
        var context = GetInMemoryDbContext();
        var repository = new EFCourseRepository(context);

        var course = new Course(
            "Advanced Strategy",
            new TimeDay(DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(3)))
        );

        await repository.AddAsync(course);
        await repository.SaveChangesAsync();

        var result = await repository.GetByIdAsync(course.Id);

        Assert.NotNull(result);
        Assert.Equal("Advanced Strategy", result.Title);
    }

    [Fact]
    public async Task Clear_ShouldRemoveAllCourses()
    {
        var context = GetInMemoryDbContext();
        var repository = new EFCourseRepository(context);

        await repository.AddAsync(new Course(
            "Course 1",
            new TimeDay(DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(2)))
        ));
        await repository.AddAsync(new Course(
            "Course 2",
            new TimeDay(DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(4)))
        ));
        await repository.SaveChangesAsync();

        repository.Clear();
        await repository.SaveChangesAsync();

        var allCourses = await repository.GetAllAsync();

        Assert.Empty(allCourses);
    }
}
