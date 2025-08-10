using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Core;
using HorsesForCourses.WebApi.Service;
using HorsesForCourses.WebApi.DTOs;

[ApiController]
[Route("courses")]
public class CourseController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CourseController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetAll()
    {
        var courses = await _courseService.GetAllAsync();
        return Ok(courses.Select(CourseMapper.ToDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseDto>> GetById(int id)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course == null)
            return NotFound();

        return Ok(CourseMapper.ToDto(course));
    }

    [HttpPost]
    public async Task<ActionResult> Add([FromBody] CreateCourseDto dto)
    {
        var course = CourseMapper.ToDomain(dto);
        await _courseService.CreateAsync(course);
        var result = CourseMapper.ToDto(course);
        return CreatedAtAction(nameof(GetById), new { id = course.Id }, result);
    }


    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] CreateCourseDto dto)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course == null)
            return NotFound();

        return NoContent();
    }
}
