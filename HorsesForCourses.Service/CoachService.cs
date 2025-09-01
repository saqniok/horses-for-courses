using HorsesForCourses.Core;
using HorsesForCourses.Service.DTOs;
using HorsesForCourses.Service.Interfaces;

namespace HorsesForCourses.Service
{
    public interface ICoachService
    {
        Task<IEnumerable<CoachSummaryResponse>> GetAllAsync();
        Task<Coach?> GetByIdAsync(int id);
        Task CreateAsync(Coach coach);
        Task UpdateAsync(Coach coach);
        Task<CoachDetailsDto?> GetDtoByIdAsync(int id);
        Task DeleteAsync(int id);
        Task RemoveSkillAsync(int id, string skill);
    }

    public class CoachService : ICoachService
    {
        private readonly ICoachRepository _coachRepository;

        public CoachService(ICoachRepository coachRepository)
        {
            _coachRepository = coachRepository;
        }

        public async Task<IEnumerable<CoachSummaryResponse>> GetAllAsync()
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

        public async Task<CoachDetailsDto?> GetDtoByIdAsync(int id)
        {
            return await _coachRepository.GetDtoByIdAsync(id);
        }

        public async Task DeleteAsync(int id)
        {
            _coachRepository.Remove(id);
            await _coachRepository.SaveChangesAsync();
        }

        public async Task RemoveSkillAsync(int id, string skill)
        {
            var coach = await _coachRepository.GetByIdAsync(id);
            if (coach == null)
                throw new InvalidOperationException($"Coach with ID {id} not found.");

            coach.RemoveSkill(skill);
            await _coachRepository.SaveChangesAsync();
        }
    }
}