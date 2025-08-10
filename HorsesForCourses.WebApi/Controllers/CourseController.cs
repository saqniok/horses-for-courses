using HorsesForCourses.Core;
using Microsoft.AspNetCore.Mvc;

public interface ICourseService
{
    IEnumerable<Course> GetAll();
    Course? GetById(int id);
    void Create(Course course);
    void Update(Course course);
    void Delete(int id);
}

[ApiController]
[Route("api/courses")]
public class CourseController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly ICoachService _coachService;

    public CourseController(ICourseService courseService, ICoachService coachService)
    {
        _courseService = courseService;
        _coachService = coachService;
    }

    // Получить все курсы
    [HttpGet]
    public IActionResult GetAll()
    {
        var courses = _courseService.GetAll();
        var result = courses.Select(CourseMapper.ToDto);
        return Ok(result);
    }

    // Получить курс по Id
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var course = _courseService.GetById(id);
        if (course == null)
            return NotFound();

        var result = CourseMapper.ToDto(course);
        return Ok(result);
    }

    // Создать курс
    [HttpPost]
    public IActionResult Create([FromBody] CreateCourseDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var course = new Course(dto.Title, new TimeDay(dto.startDate, dto.endDate));
        _courseService.Create(course);

        var result = CourseMapper.ToDto(course);
        return CreatedAtAction(nameof(GetById), new { id = course.Id }, result);
    }

    // Обновить скиллы курса
    [HttpPut("{id}/skills")]
    public IActionResult UpdateSkills(int id, [FromBody] UpdateCourseSkillsDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var course = _courseService.GetById(id);
        if (course == null)
            return NotFound();

        course.UpdateRequiredSkills(dto.Skills);
        _courseService.Update(course);

        return NoContent();
    }

    // Обновить расписание курса
    [HttpPut("{id}/schedule")]
    public IActionResult UpdateSchedule(int id, [FromBody] UpdateCourseScheduleDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var course = _courseService.GetById(id);
        if (course == null)
            return NotFound();

        _courseService.Update(course);

        return NoContent();
    }

    // Подтвердить курс
    [HttpPost("{id}/confirm")]
    public IActionResult Confirm(int id)
    {
        var course = _courseService.GetById(id);
        if (course == null)
            return NotFound();

        try
        {
            course.Confirm();
            _courseService.Update(course);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // Назначить тренера
    [HttpPost("{id}/assign-coach")]
    public IActionResult AssignCoach(int id, [FromBody] AssignCoachDto dto)
    {
        var course = _courseService.GetById(id);
        if (course == null)
            return NotFound();

        var coach = _coachService.GetById(dto.CoachId);
        if (coach == null)
            return NotFound("Coach not found");

        try
        {
            course.AssignCoach(coach);
            _courseService.Update(course);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
