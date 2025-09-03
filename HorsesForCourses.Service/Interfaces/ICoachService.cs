using HorsesForCourses.Core;
using HorsesForCourses.Service.DTOs;

namespace HorsesForCourses.Service.Interfaces;

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