using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HorsesForCourses.WebApi.Data;
using HorsesForCourses.Core;

public class CoachPersistancyTests
{
    [Fact]
    public async Task ShouldPersistData_WithLogging()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .EnableSensitiveDataLogging()
            .LogTo(Console.WriteLine, LogLevel.Information)
            .Options;

        // Create schema
        await using (var context = new AppDbContext(options))
        {
            await context.Database.EnsureCreatedAsync();
        }

        // Insert
        await using (var context = new AppDbContext(options))
        {
            context.Coaches.Add(new Coach("naam", "em@il"));

            var saved = await context.SaveChangesAsync();
            Assert.True(saved > 0, "SaveChangesAsync returned 0 — nothing was persisted.");
        }

        // Read
        await using (var context = new AppDbContext(options))
        {
            var coach = await context.Coaches.FirstOrDefaultAsync(c => c.Email == "em@il");
            Assert.NotNull(coach);
            Assert.Equal("naam", coach!.Name);
        }
    }
}
