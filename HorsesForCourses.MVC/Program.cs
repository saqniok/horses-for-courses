using HorsesForCourses.Service.Queries;
using Microsoft.EntityFrameworkCore;
using HorsesForCourses.Service;
using HorsesForCourses.Service.Data;
using HorsesForCourses.Service.Interfaces;
using HorsesForCourses.Service.Repositories;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=horsesforcourses.db"));

// Repositories & Services
builder.Services
    .AddScoped<ICoachRepository, EFCoachRepository>()
    .AddScoped<ICoachService, CoachService>()
    .AddScoped<ICourseRepository, EFCourseRepository>()
    .AddScoped<ICourseService, CourseService>()
    .AddScoped<IGetCourseSummariesQuery, GetCourseSummariesQuery>()
    .AddScoped<IGetCoachSummariesQuery, GetCoachSummariesQuery>();

// Authentication
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", o => { o.LoginPath = "/Account/Login"; });
builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
