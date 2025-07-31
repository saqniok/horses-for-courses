using System.Net;
using System.Net.Http.Json;

namespace HorsesForCourses.Tests
{
    public class CourseControllerTests : IClassFixture<CustomWebApiFactory>
    {
        private readonly CustomWebApiFactory _factory;
        private readonly HttpClient _client;

        public CourseControllerTests(CustomWebApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateCourse_ReturnsCreatedCourse()
        {
            var createDto = new CreateCourseDto("Test Course", DateTime.Today.ToString("yyyy-MM-dd"), DateTime.Today.AddDays(2).ToString("yyyy-MM-dd"));

            var response = await _client.PostAsJsonAsync("/courses", createDto);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var course = await response.Content.ReadFromJsonAsync<CourseDto>();

            Assert.NotNull(course);
            Assert.Equal(createDto.Title, course.Title);
            Assert.True(course.Id > 0);
        }

        [Fact]
        public async Task GetAllCourses_ReturnsNonEmptyList()
        {
            var createDto = new CreateCourseDto("Test2 Course", DateTime.Today.ToString("yyyy-MM-dd"), DateTime.Today.AddDays(1).ToString("yyyy-MM-dd")); ;

            await _client.PostAsJsonAsync("/courses", createDto);

            var response = await _client.GetAsync("/courses");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var courses = await response.Content.ReadFromJsonAsync<List<CourseDto>>();

            Assert.NotNull(courses);
            Assert.NotEmpty(courses);
        }

        [Fact]
        public async Task UpdateSkills_ExistingCourse_ReturnsNoContent()
        {
            var createDto = new CreateCourseDto("Test3 Course", DateTime.Today.ToString("yyyy-MM-dd"), DateTime.Today.AddDays(3).ToString("yyyy-MM-dd"));

            var createResponse = await _client.PostAsJsonAsync("/courses", createDto);
            var createdCourse = await createResponse.Content.ReadFromJsonAsync<CourseDto>();

            Assert.NotNull(createdCourse);

            var dto = new UpdateCourseSkillsDto(new List<string> { "C#", "TDD" });

            var response = await _client.PostAsJsonAsync($"/courses/{createdCourse.Id}/skills", dto);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        // [Fact]
        // public async Task AssignCoach_ValidCoach_ReturnsNoContent()
        // {
        //     var createCourseDto = new CreateCourseDto("Test4 Course", DateTime.Today, DateTime.Today.AddDays(4));

        //     var createResponse = await _client.PostAsJsonAsync("/courses", createCourseDto);
        //     var course = await createResponse.Content.ReadFromJsonAsync<CourseDto>();

        //     Assert.NotNull(course);

        //     var assignCoachDto = new AssignCoachDto(1);

        //     var response = await _client.PostAsJsonAsync($"/courses/{course.Id}/assign-coach", assignCoachDto);

        //     Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        // }


        // [Fact]
        // public async Task ConfirmCourse_ExistingCourse_ReturnsNoContent()
        // {
        //     var createDto = new CreateCourseDto("Confirm Course", DateTime.Today, DateTime.Today.AddDays(3));

        //     var createResponse = await _client.PostAsJsonAsync("/courses", createDto);
        //     createResponse.EnsureSuccessStatusCode();

        //     var createdCourse = await createResponse.Content.ReadFromJsonAsync<CourseDto>();
        //     Assert.NotNull(createdCourse);

        //     var response = await _client.PostAsync($"/courses/{createdCourse.Id}/confirm", null);

        //     Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        // }

    }
}

