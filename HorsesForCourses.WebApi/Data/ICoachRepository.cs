namespace HorsesForCourses.Core;

public interface ICoachRepository
{
    Task AddAsync(Coach coach);
    Task<Coach?> GetByIdAsync(int id);
    Task<IEnumerable<Coach>> GetAllAsync();
    void Remove(Coach coach);
    void Clear();
    Task SaveChangesAsync();
}
