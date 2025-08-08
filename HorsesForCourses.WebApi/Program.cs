using System.Text.Json.Serialization;
using HorsesForCourses.Core;
using HorsesForCourses.WebApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;



var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter(null, allowIntegerValues: false));

    options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=horsesforcourses.db"));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICoachRepository, EFCoachRepository>();
builder.Services.AddScoped<ICourseRepository, EFCourseRepository>();
builder.Services.AddScoped<CourseScheduler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // dbContext.Database.Migrate();
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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }

