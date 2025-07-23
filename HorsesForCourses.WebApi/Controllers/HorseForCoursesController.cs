using HorsesForCourses.Core;
using Microsoft.AspNetCore.Mvc;

namespace HorsesForCourses.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly ILogger<CoursesController> _logger;

        // Для упрощения — в памяти, в реальном приложении будет репозиторий/БД
        private static readonly List<Course> _courses = new();

        public CoursesController(ILogger<CoursesController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult CreateCourse([FromBody] CreateCourseRequest request)
        {
            var course = new Course(request.Title, request.Period);
            _courses.Add(course);

            _logger.LogInformation("Course created: {Title}", request.Title);

            return CreatedAtAction(nameof(GetCourse), new { title = course.Title }, course);
        }

        [HttpGet("{title}")]
        public IActionResult GetCourse(string title)
        {
            var course = _courses.FirstOrDefault(c => c.Title == title);
            if (course == null) return NotFound();

            return Ok(course);
        }

        // Добавь другие методы по необходимости...
    }

    public record CreateCourseRequest(string Title, Period Period);
}
