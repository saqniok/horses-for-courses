using HorsesForCourses.Core;
using HorsesForCourses.Service.DTOs;
using HorsesForCourses.Service.Interfaces;
using System.Linq;

namespace HorsesForCourses.Service.Queries
{
    public interface IGetCourseSummariesQuery
    {
        Task<PagedResult<CourseSummaryResponse>> All(PageRequest request);
    }

    public class GetCourseSummariesQuery : IGetCourseSummariesQuery
    {
        private readonly ICourseRepository _courseRepository;

        public GetCourseSummariesQuery(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<PagedResult<CourseSummaryResponse>> All(PageRequest request)
        {
            var courses = await _courseRepository.GetPagedAsync(request);
            var dtos = courses.Items.Select(c => new CourseSummaryResponse
            {
                Id = c.Id,
                Title = c.Title,
                StartDate = c.Period.StartDate,
                EndDate = c.Period.EndDate,
                IsConfirmed = c.IsConfirmed,
                AssignedCoachName = c.AssignedCoach?.Name,
                NumberOfLessons = c.Schedule.Count
            });

            return new PagedResult<CourseSummaryResponse>(dtos.ToList(), courses.TotalCount, courses.PageNumber, courses.PageSize);
        }
    }
}
