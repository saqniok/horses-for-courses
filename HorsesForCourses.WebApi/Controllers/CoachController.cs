using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Core;

[ApiController]
[Route("coaches")]
public class CoachController : ControllerBase
{
    private readonly ICoachRepository _repository;

    public CoachController(ICoachRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public ActionResult Add([FromBody] CreateCoachDto dto)
    {
        var coach = new Coach(dto.Name, dto.Email);
        _repository.Add(coach);

        return CreatedAtAction(nameof(GetById), new { id = coach.Id }, CoachMapper.ToCoachDetailsDto(coach));
    }

    [HttpGet("{id}")]
    public ActionResult<CoachDetailsDto> GetById(int id)
    {
        var coach = _repository.GetById(id);

        if (coach == null)
            return NotFound();

        return Ok(CoachMapper.ToCoachDetailsDto(coach));
    }

    [HttpGet]
    public ActionResult<IEnumerable<CoachSummaryDto>> GetAll()
    {
        var coaches = _repository.GetAll();

        return Ok(coaches.Select(CoachMapper.ToCoachSummaryDto).ToList());
    }


    [HttpPost("{id}/skills")]
    public ActionResult UpdateCoachSkills(int id, [FromBody] UpdateCoachSkillsDto dto)
    {
        var coach = _repository.GetById(id);

        if (coach == null)
            return NotFound();

        coach.UpdateSkills(dto.Skills);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        if (!_repository.Remove(id))
            return NotFound();

        return NoContent();
    }
}
