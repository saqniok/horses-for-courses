using System.Net.Http.Json;
using HorsesForCourses.Blazor.Models;

namespace HorsesForCourses.Blazor.Services;

public class CourseService
{
    private readonly HttpClient _http;

    public CourseService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<CourseDto>> GetCoursesAsync()
    {
        return await _http.GetFromJsonAsync<List<CourseDto>>("api/courses") ?? new List<CourseDto>();
    }
}
