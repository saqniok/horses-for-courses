namespace HorsesForCourses.Core;

public interface ICoachRepository
{
    void Add(Coach coach);
    Coach? GetById(int id);
    IEnumerable<Coach> GetAll();
    bool Remove(int id);
    void Clear();
    void SaveChanges();
}
