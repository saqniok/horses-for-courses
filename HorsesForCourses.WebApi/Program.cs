using System.Text.Json.Serialization;
using HorsesForCourses.Core;
using HorsesForCourses.Service.Data;
using HorsesForCourses.Service;
using HorsesForCourses.Service.Interfaces;
using HorsesForCourses.Service.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter(null, allowIntegerValues: false));

    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=horsesforcourses.db"));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();


builder.Services
    .AddScoped<ICoachRepository, EFCoachRepository>()
    .AddScoped<ICoachService, CoachService>()
    .AddScoped<ICourseRepository, EFCourseRepository>()
    .AddScoped<ICourseService, CourseService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler(appBuilder =>
    {
        appBuilder.Run(async context =>
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Title = "Er is een fout opgetreden",
                Status = 500,
                Detail = "Er ging iets mis tijdens het verwerken van je verzoek."
            };

            await context.Response.WriteAsJsonAsync(problem);
        });
    });
}

app.UseCors();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }