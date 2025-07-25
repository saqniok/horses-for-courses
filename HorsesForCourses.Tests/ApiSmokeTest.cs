using System.Net;

namespace HorsesForCourses.Tests;

public class ApiSmokeTests : IClassFixture<CustomWebAppFactory>
{
    private readonly HttpClient _client;

    public ApiSmokeTests(CustomWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Swagger_Should_Return_OK()
    {
        var response = await _client.GetAsync("/swagger/index.html");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
