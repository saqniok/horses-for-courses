using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Core;
using HorsesForCourses.WebApi.Service;
using HorsesForCourses.WebApi.DTOs;

[ApiController]
[Route("courses")]
public class CourseController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly ICoachService _coachService;

    public CourseController(ICourseService courseService, ICoachService coachService)
    {
        _courseService = courseService;
        _coachService = coachService;
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

    [HttpPut("{id}/skills")]
    public async Task<ActionResult> UpdateSkills(int id, [FromBody] IEnumerable<string> skills)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course == null)
            return NotFound();

        try
        {
            course.UpdateRequiredSkills(skills);
            await _courseService.UpdateAsync(course);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

        return NoContent();
    }
    [HttpPost("{id}/timeslots")]
    public async Task<ActionResult> AddTimeSlot(int id, [FromBody] TimeSlotDto dto)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course == null)
            return NotFound();

        course.AddTimeSlot(new TimeSlot(dto.Day, dto.Start, dto.End));

        await _courseService.UpdateAsync(course);

        return NoContent();
    }

    [HttpPost("{id}/confirm")]
    public async Task<ActionResult> ConfirmCourse(int id)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course == null)
            return NotFound();

        course.Confirm();
        await _courseService.UpdateAsync(course);

        return NoContent();
    }

    [HttpPost("{id}/assigncoach")]
    public async Task<ActionResult> AssignCoach(int id, [FromBody] AssignCoachDto dto)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course == null)
            return NotFound();

        var coach = await _coachService.GetByIdAsync(dto.CoachId);

        if (coach == null)
            return NotFound("Coach not found.");

        course.AssignCoach(coach);

        await _courseService.UpdateAsync(course);

        return NoContent();
    }

    [HttpGet("paged")]
    public async Task<ActionResult<PagedResult<Course>>> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken ct = default)
    {
        var request = new PageRequest(page, pageSize); 
        var result = await _courseService.GetPagedAsync(request, ct);
        return Ok(result);
    }

}
