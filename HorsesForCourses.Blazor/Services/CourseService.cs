using System.Net.Http.Json;
using HorsesForCourses.Blazor.Dtos;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HorsesForCourses.Blazor.Services;

public class CourseService : ICourseService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public CourseService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        _jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public async Task<List<CourseDto>> GetCoursesAsync()
    {
        var response = await _httpClient.GetAsync("courses");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<CourseDto>>(_jsonSerializerOptions);
        return result ?? new List<CourseDto>();
    }

    public async Task DeleteCourseAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"courses/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task ConfirmCourseAsync(int id)
    {
        var response = await _httpClient.PostAsync($"courses/{id}/confirm", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task AddCourseAsync(CreateCourseRequestDto course)
    {
        var response = await _httpClient.PostAsJsonAsync("courses", course, _jsonSerializerOptions);
        response.EnsureSuccessStatusCode();
    }
    
    public async Task UpdateCourseAsync(int id, CourseDto course)
    {
        var response = await _httpClient.PutAsJsonAsync($"courses/{id}", course, _jsonSerializerOptions);
        response.EnsureSuccessStatusCode();
    }
}
