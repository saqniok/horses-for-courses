using HorsesForCourses.Core;

public class InMemoryCoachRepository
{
    private readonly Dictionary<Guid, Coach> _coaches = new();

    public void Add(Coach coach)
    {
        _coaches[coach.Id] = coach;
    }

    public Coach? GetById(Guid id)
    {
        return _coaches.TryGetValue(id, out var coach) ? coach : null;
    }

    public IEnumerable<Coach> GetAll()
    {
        return _coaches.Values;
    }

    public bool Remove(Guid id)
    {
        return _coaches.Remove(id);
    }

    public void Clear()
    {
        _coaches.Clear();
    }
}
