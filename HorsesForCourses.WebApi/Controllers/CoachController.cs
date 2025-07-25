using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Core;

[ApiController]
[Route("coaches")]
public class CoachController : ControllerBase
{
    private readonly InMemoryCoachRepository _repository;

    public CoachController(InMemoryCoachRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{id}")]
    public ActionResult<CoachDto> GetById(Guid id)
    {
        var coach = _repository.GetById(id);
        if (coach == null) return NotFound();

        return Ok(CoachMapper.ToDto(coach));
    }

    [HttpGet]
    public ActionResult<IEnumerable<CoachDto>> GetAll()
    {
        var coaches = _repository.GetAll();
        return Ok(coaches.Select(CoachMapper.ToDto).ToList());
    }

[HttpPost]
public ActionResult Add([FromBody] CoachDto dto)
{
    var coach = new Coach(dto.Name, dto.Email);
    _repository.Add(coach);

    return CreatedAtAction(nameof(GetById), new { id = coach.Id }, CoachMapper.ToDto(coach));
}

    [HttpPost("{id}/skills")]
    public ActionResult UpdateCoachSkills(Guid id, [FromBody] UpdateCoachSkillsDto dto)
    {
        var coach = _repository.GetById(id);
        if (coach == null) return NotFound();

        foreach (var skill in coach.Skills.ToList())
            coach.RemoveSkill(skill);

        foreach (var skill in dto.Skills)
            coach.AddSkill(skill);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(Guid id)
    {
        if (!_repository.Remove(id))
            return NotFound();

        return NoContent();
    }
}
