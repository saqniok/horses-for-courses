
using System.Net;
using System.Text.Json;
using HorsesForCourses.Core;

namespace HorsesForCourses.Tests.Integration;

public class CoachApiTests : IClassFixture<CustomWebApiFactory>
{
    private readonly HttpClient _client;

    public CoachApiTests(CustomWebApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllCoaches_ReturnsSuccessAndJson()
    {
        var response = await _client.GetAsync("/coaches");
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();

        var coaches = JsonSerializer.Deserialize<List<CoachSummaryDto>>(body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(coaches);
        Assert.NotEmpty(coaches);

        Assert.Contains(coaches, c => c.Name == "John Doe");
        Assert.Contains(coaches, c => c.Name == "Jane Smith");
    }

    [Fact]
    public async Task GetCoachById_Returns404_WhenNotFound()
    {
        var response = await _client.GetAsync($"/coaches/{int.MaxValue}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }


}
