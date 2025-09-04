using HorsesForCourses.Core;
using HorsesForCourses.Service.DTOs;

namespace HorsesForCourses.Service.Interfaces;

public interface ICourseService
{
    Task<IEnumerable<CourseDto>> GetAllAsync();
    Task<Course?> GetByIdAsync(int id);
    Task<CourseDto?> GetDtoByIdAsync(int id);
    Task CreateAsync (Course course);
    Task UpdateAsync (Course course);
    Task DeleteAsync (int id);
    Task<PagedResult<Course>> GetPagedAsync(PageRequest request, CancellationToken ct = default);
}
