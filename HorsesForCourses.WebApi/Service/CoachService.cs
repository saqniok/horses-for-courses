using HorsesForCourses.Core;

public interface ICoachService
{
    Task<IEnumerable<Coach>> GetAllAsync();
    Task<Coach?> GetByIdAsync(int id);
    Task CreateAsync(Coach coach);
    Task UpdateAsync(Coach coach);
}

namespace HorsesForCourses.WebApi.Service
{
    public class CoachService : ICoachService
    {
        private readonly ICoachRepository _coachRepository;

        public CoachService(ICoachRepository coachRepository)
        {
            _coachRepository = coachRepository;
        }

        public async Task<IEnumerable<Coach>> GetAllAsync()
        {
            return await _coachRepository.GetAllAsync();
        }

        public async Task<Coach?> GetByIdAsync(int id)
        {
            return await _coachRepository.GetByIdAsync(id);
        }

        public async Task CreateAsync(Coach coach)
        {
            await _coachRepository.AddAsync(coach);
            await _coachRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(Coach coach)
        {
            await _coachRepository.SaveChangesAsync();
        }
    }
}