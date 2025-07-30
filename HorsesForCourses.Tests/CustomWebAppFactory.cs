using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using HorsesForCourses.Core;


namespace HorsesForCourses.Tests;

public class CustomWebAppFactory : WebApplicationFactory<Program>
{

    private InMemoryCoachRepository? _repository;
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(InMemoryCoachRepository));
            if (descriptor != null)
                services.Remove(descriptor);

            _repository = new InMemoryCoachRepository();
            _repository.Add(new Coach("John Doe", "john@example.com"));
            _repository.Add(new Coach("Jane Smith", "jane@example.com"));

            services.AddSingleton<InMemoryCoachRepository>(_repository);
        });

    }

    public void ClearRepository()
    {
        _repository?.Clear();
        _repository?.Add(new Coach("John Doe", "john@example.com"));
        _repository?.Add(new Coach("Jane Smith", "jane@example.com"));
    }
}
