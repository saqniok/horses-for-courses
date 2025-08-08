using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi.Data
{
    public class EFCoachRepository : ICoachRepository
    {
        private readonly AppDbContext _context;

        public EFCoachRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(Coach coach)
        {
            _context.Coaches.Add(coach);
        }

        public Coach? GetById(int id)
        {
            return _context.Coaches.Find(id);
        }

        public IEnumerable<Coach> GetAll()
        {
            return _context.Coaches.ToList();
        }

        public bool Remove(int id)
        {
            var coach = _context.Coaches.Find(id);
            
            if (coach == null)
                return false;

            _context.Coaches.Remove(coach);
            return true;
        }

        public void Clear()
        {
            _context.Coaches.RemoveRange(_context.Coaches);
        }

        public void SaveChanges()
        {
             _context.SaveChanges();
        }
    }
}
