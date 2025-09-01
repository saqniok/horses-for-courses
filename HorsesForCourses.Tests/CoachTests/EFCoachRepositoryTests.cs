using HorsesForCourses.Core;
using HorsesForCourses.Service.Data;
using HorsesForCourses.Service.Repositories;
using Microsoft.EntityFrameworkCore;


public class EFCoachRepositoryTests
{
    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAndGetById_ShouldWorkCorrectly()
    {
        await using (var context = CreateDbContext())
        {
            var repo = new EFCoachRepository(context);

            var coach = new Coach("John Doe", "john@example.com");
            await repo.AddAsync(coach);
            await repo.SaveChangesAsync();

            var fetched = await repo.GetByIdAsync(coach.Id);

            Assert.NotNull(fetched);
            Assert.Equal("John Doe", fetched!.Name);
            Assert.Equal("john@example.com", fetched.Email);
        }
    }

    [Fact]
    public async Task Remove_ShouldRemoveCoach()
    {

        await using var context = CreateDbContext();
        var repo = new EFCoachRepository(context);

        var coach = new Coach("Jane Doe", "jane@example.com");
        await repo.AddAsync(coach);
        await repo.SaveChangesAsync();

        repo.Remove(coach.Id);
        await repo.SaveChangesAsync();

        var fetched = await repo.GetByIdAsync(coach.Id);

        Assert.Null(fetched);
    }

    [Fact]
    public async Task Clear_ShouldRemoveAllCoaches()
    {

        await using var context = CreateDbContext();
        var repo = new EFCoachRepository(context);

        await repo.AddAsync(new Coach("A", "a@example.com"));
        await repo.AddAsync(new Coach("B", "b@example.com"));
        await repo.SaveChangesAsync();

        repo.Clear();
        await repo.SaveChangesAsync();

        var allCoaches = await repo.GetAllAsync();

        Assert.Empty(allCoaches);
    }
}
