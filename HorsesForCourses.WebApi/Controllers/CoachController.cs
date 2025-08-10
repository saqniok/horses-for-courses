using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Core;

public interface ICoachService
{
    IEnumerable<Coach> GetAll();
    Coach? GetById(int id);
    void Create(Coach coach);
    void Update(Coach coach);
    // и т.д.
}

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
    public ActionResult Add([FromBody] CreateCoachDto dto)
    {
        var coach = CoachMapper.ToDomain(dto);
        _coachService.Create(coach);

        var result = CoachMapper.ToCoachSummaryDto(coach);
        return CreatedAtAction(nameof(GetById), new { id = coach.Id }, result);
    }

    [HttpGet("{id}")]
    public ActionResult<CoachDetailsDto> GetById(int id)
    {
        var coach = _coachService.GetById(id);

        if (coach == null)
            return NotFound();

        return Ok(CoachMapper.ToCoachDetailsDto(coach));
    }

    [HttpGet]
    public ActionResult<IEnumerable<CoachSummaryDto>> GetAll()
    {
        var coaches = _coachService.GetAll();

        return Ok(coaches.Select(CoachMapper.ToCoachSummaryDto));
    }


    [HttpPost("{id}/skills")]
    public ActionResult UpdateCoachSkills(int id, [FromBody] UpdateCoachSkillsDto dto)
    {
        var coach = _coachService.GetById(id);

        if (coach == null)
            return NotFound();

        CoachMapper.UpdateSkills(coach, dto);
        _coachService.Update(coach);

        return NoContent();
    }
}
