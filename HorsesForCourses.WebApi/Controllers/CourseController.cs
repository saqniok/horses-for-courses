using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Core;
using HorsesForCourses.WebApi.Service;
using HorsesForCourses.WebApi.DTOs;

namespace HorsesForCourses.WebApi.Controllers;

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
        return Ok(courses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseDto>> GetById(int id)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course == null)
            return NotFound();

        return Ok(course);
    }


    [HttpPost]
    public async Task<ActionResult> Add([FromBody] CreateCourseRequest dto)
    {
        var course = new Course(dto.Title, new TimeDay(dto.startDate, dto.endDate));
        await _courseService.CreateAsync(course);

        var resultDto = new CourseDto(
            course.Id,
            course.Title,
            course.Period.StartDate,
            course.Period.EndDate);

        return CreatedAtAction(nameof(GetById), new { id = course.Id }, resultDto);
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
    public async Task<ActionResult> UpdateTimeSlots(int id, [FromBody] List<TimeSlotDto> dtos)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course == null)
            return NotFound();

        var newTimeSlots = dtos.Select(dto => new TimeSlot(dto.Day, dto.Start, dto.End));
        course.UpdateTimeSlot(newTimeSlots);

        await _courseService.UpdateAsync(course);

        return NoContent();
    }
  

    [HttpPost("{id}/confirm")]
    public async Task<ActionResult> ConfirmCourse(int id)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course == null)
            return NotFound();

        try
        {
            // Check if course can be confirmed (has schedule)
            if (!course.Schedule.Any())
            {
                return BadRequest("Cannot confirm course without any lessons. Please add schedule first.");
            }

            course.Confirm();
            await _courseService.UpdateAsync(course);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

        return NoContent();
    }

    [HttpPost("{id}/assigncoach")]
    public async Task<ActionResult> AssignCoach(int id, [FromBody] AssignCoachRequest dto)
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
    public async Task<ActionResult<PagedResult<CourseDto>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken ct = default)
    {
        var request = new PageRequest(page, pageSize);
        var result = await _courseService.GetPagedAsync(request, ct);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] CourseDto dto)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course == null)
            return NotFound();

        try
        {
            // Update title
            course.UpdateTitle(dto.Title);
            
            // Note: Period (dates) cannot be updated as it's read-only in the domain model
            // If date changes are needed, this would require domain model changes
            
            // Update required skills
            if (dto.RequiredSkills != null)
            {
                course.UpdateRequiredSkills(dto.RequiredSkills);
            }
            
            // Update schedule/time slots
            if (dto.Schedule != null)
            {
                var newTimeSlots = dto.Schedule.Select(ts => new TimeSlot(ts.Day, ts.Start, ts.End));
                course.UpdateTimeSlot(newTimeSlots);
            }
            
            await _courseService.UpdateAsync(course);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course == null)
            return NotFound();

        await _courseService.DeleteAsync(id);
        return NoContent();
    }
}
