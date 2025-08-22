using HorsesForCourses.WebApi.DTOs;

namespace HorsesForCourses.Core;

public interface ICoachRepository
{
    Task AddAsync(Coach coach);
    Task<Coach?> GetByIdAsync(int id);
    Task<CoachDetailsDto?> GetDtoByIdAsync(int id);
    Task<IEnumerable<CoachSummaryResponse>> GetAllAsync();
    void Remove(int id);
    void Clear();
    void Update(Coach coach);
    Task SaveChangesAsync();
    Task DeleteAsync(int id);
}

public interface ICourseRepository
{
    Task AddAsync(Course course);
    Task<Course?> GetByIdAsync(int id);
    Task<IEnumerable<CourseDto>> GetAllAsync();
    Task<PagedResult<Course>> GetPagedAsync(PageRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id);
    void Clear();
    void Update(Course course);
    Task SaveChangesAsync();
}