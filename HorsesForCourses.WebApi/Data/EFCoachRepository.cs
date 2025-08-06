using HorsesForCourses.Core;
using HorsesForCourses.WebApi.Data;
using Microsoft.EntityFrameworkCore;

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
            _context.SaveChanges();
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
            {
                return false;
            }
            _context.Coaches.Remove(coach);
            _context.SaveChanges();
            return true;
        }

        public void Clear()
        {
            _context.Coaches.RemoveRange(_context.Coaches);
            _context.SaveChanges();
        }
    }
}
