// namespace HorsesForCourses.Core;

// public class InMemoryCoachRepository : ICoachRepository
// {
//     private readonly Dictionary<int, Coach> _coaches = new();
//     private int _nextId = 1;

//     public void Add(Coach coach)
//     {
//         coach.Id = _nextId++;

//         _coaches[coach.Id] = coach;
//     }

//     public Coach? GetById(int id)
//     {
//         return _coaches.TryGetValue(id, out var coach) ? coach : null;
//     }

//     public IEnumerable<Coach> GetAll()
//     {
//         return _coaches.Values;
//     }

//     public bool Remove(int id)
//     {
//         return _coaches.Remove(id);
//     }

//     public void Clear()
//     {
//         _coaches.Clear();
//         _nextId = 1;
//     }

//     public void SaveChanges()
//     {

//     }
// }
