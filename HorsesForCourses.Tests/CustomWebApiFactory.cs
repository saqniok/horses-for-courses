using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using HorsesForCourses.Core;

namespace HorsesForCourses.Tests
{
    public class CustomWebApiFactory : WebApplicationFactory<Program>
    {
        private readonly InMemoryCoachRepository _coachRepository = new();
        private readonly InMemoryCourseRepository _courseRepository = new();
        private readonly CourseScheduler _courseScheduler;

        public CustomWebApiFactory()
        {
            _courseScheduler = new CourseScheduler();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");

            builder.ConfigureServices(services =>
            {

                var coachDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(InMemoryCoachRepository));
                if (coachDescriptor != null)
                    services.Remove(coachDescriptor);

                var courseDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(InMemoryCourseRepository));
                if (courseDescriptor != null)
                    services.Remove(courseDescriptor);

                var schedulerDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(CourseScheduler));
                if (schedulerDescriptor != null)
                    services.Remove(schedulerDescriptor);

                services.AddSingleton(_coachRepository);
                services.AddSingleton(_courseRepository);
                services.AddSingleton(_courseScheduler);

                ClearRepository();
            });
        }

        public void ClearRepository()
        {
            _coachRepository.Clear();
            _coachRepository.Add(new Coach("John Doe", "john@example.com"));
            _coachRepository.Add(new Coach("Jane Smith", "jane@example.com"));

            _courseRepository.Clear();
        }
    }
}
