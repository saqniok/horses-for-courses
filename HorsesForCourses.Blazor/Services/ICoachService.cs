using HorsesForCourses.Blazor.Dtos;

namespace HorsesForCourses.Blazor.Services;

public interface ICoachService
{
    Task<List<CoachSummaryResponse>> GetCoachesAsync();
    Task<CoachDetailsDto> GetCoachDetailsAsync(int id);
    Task AddCoachAsync(CreateCoachRequest request);
    Task UpdateCoachAsync(int id, CoachDetailsDto coach);
    Task UpdateCoachSkillsAsync(int id, UpdateCoachSkillsDto skills);
    Task DeleteCoachAsync(int id);
}
