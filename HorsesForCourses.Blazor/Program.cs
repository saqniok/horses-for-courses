using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using HorsesForCourses.Blazor;
using HorsesForCourses.Blazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri("http://localhost:5121") });

builder.Services.AddScoped<ICoachService, CoachService>();
builder.Services.AddScoped<ICourseService, CourseService>();

await builder.Build().RunAsync();