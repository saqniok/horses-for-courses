using HorsesForCourses.Service.Queries;
using Microsoft.EntityFrameworkCore;
using HorsesForCourses.Service;
using HorsesForCourses.Service.Data;
using HorsesForCourses.Service.Interfaces;
using HorsesForCourses.Service.Repositories;
using HorsesForCourses.Core; // Added for IPasswordHasher and Pbkdf2PasswordHasher


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
    .AddScoped<IGetCoachSummariesQuery, GetCoachSummariesQuery>()
    .AddScoped<IUserRepository, EFUserRepository>() 
    .AddScoped<IUserService, UserService>() 
    .AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>(); // check this after

// Authentication
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", o => { o.LoginPath = "/Account/Login"; })
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    })
    .AddFacebook(facebookOptions =>
    {
        facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
        facebookOptions.ClientSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
    })
    .AddGitHub(githubOptions =>
    {
        githubOptions.ClientId = builder.Configuration["Authentication:GitHub:ClientId"];
        githubOptions.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"];
    });
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

// Apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
