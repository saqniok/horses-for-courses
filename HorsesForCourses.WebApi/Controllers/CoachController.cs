using HorsesForCourses.Core;
using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.WebApi.Service;
namespace HorsesForCourses.WebApi.Controllers;

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
    public async Task<ActionResult<CoachSummaryResponse>> Add([FromBody] CreateCoachRequest dto)
    {
        var coach = new Coach(dto.Name, dto.Email);
        await _coachService.CreateAsync(coach);

        var resultDto = new CoachSummaryResponse
        {
            Id = coach.Id,
            Name = coach.Name,
            Email = coach.Email,
            NumberOfCoursesAssignedTo = coach.AssignedCourses.Count
        };

        return CreatedAtAction(nameof(GetById), new { id = coach.Id }, resultDto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CoachSummaryResponse>>> GetAll()
    {
        var coaches = await _coachService.GetAllAsync();
        return Ok(coaches);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CoachDetailsDto>> GetById(int id)
    {
        var coach = await _coachService.GetDtoByIdAsync(id);
        if (coach == null)
            return NotFound();

        return Ok(coach);
    }

    [HttpPost("{id}/skills")]
    public async Task<ActionResult> UpdateCoachSkills(int id, [FromBody] UpdateCoachSkillsDto dto)
    {
        var coach = await _coachService.GetByIdAsync(id);
        if (coach == null)
            return NotFound();
        coach.UpdateSkills(dto.Skills);
        await _coachService.UpdateAsync(coach);

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] CoachDetailsDto dto)
    {
        var coach = await _coachService.GetByIdAsync(id);
        if (coach == null)
            return NotFound();

        coach.UpdateDetails(dto.Name, dto.Email);
        await _coachService.UpdateAsync(coach);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var coach = await _coachService.GetByIdAsync(id);
        if (coach == null)
            return NotFound();

        await _coachService.DeleteAsync(id);

        return NoContent();
    }
}
