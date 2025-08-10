using HorsesForCourses.WebApi.DTOs;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("coaches")]
public class CoachController : ControllerBase
{
    private readonly ICoachService _coachService;

    public CoachController(ICoachService coachService)
    {
        _coachService = coachService;
    }

    [HttpPost]
    public async Task<ActionResult> Add([FromBody] CreateCoachDto dto)
    {
        var coach = CoachMapper.ToDomain(dto);
        await _coachService.CreateAsync(coach);

        var result = CoachMapper.ToCoachSummaryDto(coach);
        return CreatedAtAction(nameof(GetById), new { id = coach.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CoachSummaryDto>>> GetAll()
    {
        var coaches = await _coachService.GetAllAsync();
        return Ok(coaches.Select(CoachMapper.ToCoachSummaryDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CoachDetailsDto>> GetById(int id)
    {
        var coach = await _coachService.GetByIdAsync(id);
        if (coach == null)
            return NotFound();

        return Ok(CoachMapper.ToCoachDetailsDto(coach));
    }

    [HttpPost("{id}/skills")]
    public async Task<ActionResult> UpdateCoachSkills(int id, [FromBody] UpdateCoachSkillsDto dto)
    {
        var coach = await _coachService.GetByIdAsync(id);
        if (coach == null)
            return NotFound();

        CoachMapper.UpdateSkills(coach, dto);
        await _coachService.UpdateAsync(coach);

        return NoContent();
    }
}
