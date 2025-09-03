using HorsesForCourses.Core;
using HorsesForCourses.Service.DTOs;
using HorsesForCourses.Service.Interfaces;
using HorsesForCourses.Service.Data;

namespace HorsesForCourses.Service
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;

        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<IEnumerable<CourseDto>> GetAllAsync()
        {
            return await _courseRepository.GetAllAsync();
        }

        public async Task<Course?> GetByIdAsync(int id)
        {
            return await _courseRepository.GetByIdAsync(id);
        }

        public async Task CreateAsync(Course course)
        {
            await _courseRepository.AddAsync(course);
            await _courseRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(Course course)
        {
            _courseRepository.Update(course);
            await _courseRepository.SaveChangesAsync();
        }

        public async Task<PagedResult<Course>> GetPagedAsync(PageRequest request, CancellationToken ct = default)
        {
            return await _courseRepository.GetPagedAsync(request, ct);
        }

        public async Task DeleteAsync(int id)
        {
            await _courseRepository.DeleteAsync(id);
            await _courseRepository.SaveChangesAsync();
        }
    }
}