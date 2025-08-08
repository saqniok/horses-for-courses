
using System.Net;
using System.Text.Json;
using System.Text;


namespace HorsesForCourses.Tests.Integration;

public class CoachControllerIntegrationTests : IClassFixture<CustomWebApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly CustomWebApiFactory _factory;

    public CoachControllerIntegrationTests(CustomWebApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        _factory.ClearRepository();
        await Task.CompletedTask;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetAllCoaches_ReturnsSuccessAndJson()
    {

        var response = await _client.GetAsync("/coaches");

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var coaches = JsonSerializer.Deserialize<List<CoachSummaryDto>>(body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(coaches);

        Assert.Contains(coaches, c => c.Name == "John Doe");
        Assert.Contains(coaches, c => c.Name == "Jane Smith");
    }

    [Fact]
    public async Task GetCoachById_ReturnsSuccess_WhenFound()
    {
        var response = await _client.GetAsync("/coaches/1");

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var coach = JsonSerializer.Deserialize<CoachDetailsDto>(body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(coach);
        Assert.Equal(1, coach.Id);
        Assert.Equal("John Doe", coach.Name);
        Assert.Equal("john@example.com", coach.Email);
    }

    [Fact]
    public async Task GetCoachById_Returns404_WhenNotFound()
    {
        var response = await _client.GetAsync($"/coaches/{int.MaxValue}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddCoach_ReturnsCreatedAtAction_WithNewCoach()
    {
        var newCoachDto = new CreateCoachDto { Name = "New", Email = "newtest@example.com" };
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(newCoachDto),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _client.PostAsync("/coaches", jsonContent);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var newCoachIdString = response.Headers.Location?.Segments.LastOrDefault();
        Assert.NotNull(newCoachIdString);
        Assert.True(int.TryParse(newCoachIdString, out int newCoachId));
        Assert.Equal(3, newCoachId);


        var getResponse = await _client.GetAsync(response.Headers.Location);
        getResponse.EnsureSuccessStatusCode();

        var createdCoach = JsonSerializer.Deserialize<CoachDetailsDto>(
            await getResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(createdCoach);
        Assert.Equal("New", createdCoach.Name);
        Assert.Equal("newtest@example.com", createdCoach.Email);
        Assert.Equal(newCoachId, createdCoach.Id);
    }

    [Fact]
    public async Task UpdateCoachSkills_ReturnsNoContent_WhenCoachAndSkillsExist()
    {

        var updateSkillsDto = new UpdateCoachSkillsDto { Skills = new List<string> { "Leadership", "Team Building" } };
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(updateSkillsDto),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _client.PostAsync("/coaches/1/skills", jsonContent);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync("/coaches/1");
        getResponse.EnsureSuccessStatusCode();
        var coach = JsonSerializer.Deserialize<CoachDetailsDto>(
            await getResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(coach);
        Assert.Contains("Leadership", coach.Skills);
        Assert.Contains("Team Building", coach.Skills);
        Assert.Equal(2, coach.Skills.Count);
    }

    [Fact]
    public async Task UpdateCoachSkills_Returns404_WhenCoachDoesNotExist()
    {

        var updateSkillsDto = new UpdateCoachSkillsDto { Skills = new List<string> { "Skill A" } };
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(updateSkillsDto),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _client.PostAsync($"/coaches/{int.MaxValue}/skills", jsonContent);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    // [Fact]
    // public async Task DeleteCoach_ReturnsNoContent_WhenCoachExists()
    // {

    //     var deleteResponse = await _client.DeleteAsync($"/coaches/1");

    //     Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

    //     var getResponse = await _client.GetAsync($"/coaches/1");
    //     Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

    //     var getAllResponse = await _client.GetAsync("/coaches");
    //     getAllResponse.EnsureSuccessStatusCode();
    //     var remainingCoaches = JsonSerializer.Deserialize<List<CoachSummaryDto>>(
    //         await getAllResponse.Content.ReadAsStringAsync(),
    //         new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
    //     );
    //     Assert.NotNull(remainingCoaches);
    //     Assert.Single(remainingCoaches);
    //     Assert.Equal("Jane Smith", remainingCoaches.First().Name);
    // }

    // [Fact]
    // public async Task DeleteCoach_Returns404_WhenCoachDoesNotExist()
    // {
    //     var response = await _client.DeleteAsync($"/coaches/{int.MaxValue}");

    //     Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    // }
}