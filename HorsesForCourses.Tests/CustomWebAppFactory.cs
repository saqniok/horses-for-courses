using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using HorsesForCourses.Core;

namespace HorsesForCourses.Tests;

public class CustomWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(InMemoryCoachRepository));
            if (descriptor != null)
                services.Remove(descriptor);

            var testRepo = new InMemoryCoachRepository();
            testRepo.Add(new Coach("John Doe", "john@example.com"));
            testRepo.Add(new Coach("Jane Smith", "jane@example.com"));

            services.AddSingleton<InMemoryCoachRepository>(testRepo);
        });

    }
}
