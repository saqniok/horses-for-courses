namespace HorsesForCourses.Core;

public interface ICoachRepository
{
    Task AddAsync(Coach coach);
    Task<Coach?> GetByIdAsync(int id);
    Task<IEnumerable<Coach>> GetAllAsync();
    void Remove(int id);
    void Clear();
    void Update(Coach coach);
    Task SaveChangesAsync();
}

public interface ICourseRepository
{
    Task AddAsync(Course course);
    Task <Course?> GetByIdAsync(int id);
    Task <IEnumerable<Course>> GetAllAsync();
    Task<PagedResult<Course>> GetPagedAsync(PageRequest request, CancellationToken ct = default);
    void Clear();
    void Update(Course course);
    Task SaveChangesAsync();
}


