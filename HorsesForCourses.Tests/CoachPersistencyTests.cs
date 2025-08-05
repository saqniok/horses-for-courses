using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using HorsesForCourses.Core;
using HorsesForCourses.WebApi.Data;

namespace HorsesForCourses.Tests;

public class CoachPersistancyTests
{
    [Fact]
    public async Task ShouldPersistData()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using (var context = new AppDbContext(options))
        {
            await context.Database.EnsureCreatedAsync();
        }

        await using (var context = new AppDbContext(options))
        {
            context.Coaches.Add(new Coach("naam", "em@il"));
            await context.SaveChangesAsync();
        }

        await using (var context = new AppDbContext(options))
        {
            var coach = await context.Coaches.FirstOrDefaultAsync(c => c.Email == "em@il");

            Assert.NotNull(coach);
            Assert.Equal("naam", coach!.Name);
        }
    }
}
