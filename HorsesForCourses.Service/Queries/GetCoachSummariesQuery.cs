using HorsesForCourses.Core;
using HorsesForCourses.Service.DTOs;
using HorsesForCourses.Service.Interfaces;

namespace HorsesForCourses.Service.Queries
{
    public interface IGetCoachSummariesQuery
    {
        Task<PagedResult<CoachSummaryResponse>> All(PageRequest request);
    }

    public class GetCoachSummariesQuery : IGetCoachSummariesQuery
    {
        private readonly ICoachRepository _coachRepository;

        public GetCoachSummariesQuery(ICoachRepository coachRepository)
        {
            _coachRepository = coachRepository;
        }

        public async Task<PagedResult<CoachSummaryResponse>> All(PageRequest request)
        {
            return await _coachRepository.GetPagedAsync(request);
        }
    }
}