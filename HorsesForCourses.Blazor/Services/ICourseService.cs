using HorsesForCourses.Blazor.Dtos;

namespace HorsesForCourses.Blazor.Services;

public interface ICourseService
{
    Task<List<CourseDto>> GetCoursesAsync();
}
