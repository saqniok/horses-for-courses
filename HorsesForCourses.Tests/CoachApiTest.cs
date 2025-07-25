
using System.Net;
using System.Text.Json;

namespace HorsesForCourses.Tests.Integration;

public class CoachApiTests : IClassFixture<CustomWebAppFactory>
{
    private readonly HttpClient _client;

    public CoachApiTests(CustomWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllCoaches_ReturnsSuccessAndJson()
    {
        var response = await _client.GetAsync("/coaches");
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        Console.WriteLine(body); 
        var coaches = JsonSerializer.Deserialize<List<CoachDto>>(body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(coaches);
        Assert.NotEmpty(coaches); 

        Assert.Contains(coaches, c => c.Name == "John Doe");
    }

    [Fact]
    public async Task GetCoachById_Returns404_WhenNotFound()
    {
        var response = await _client.GetAsync($"/coaches/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
