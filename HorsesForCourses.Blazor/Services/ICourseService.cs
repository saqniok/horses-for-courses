using HorsesForCourses.Blazor.Dtos;

namespace HorsesForCourses.Blazor.Services;

public interface ICourseService
{
    Task<List<CourseDto>> GetCoursesAsync();
    Task DeleteCourseAsync(int id);
    Task ConfirmCourseAsync(int id);
    Task AddCourseAsync(CreateCourseRequestDto course);
    Task UpdateCourseAsync(int id, CourseDto course);
}


