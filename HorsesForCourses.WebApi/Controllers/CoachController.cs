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
    public ActionResult<Coach> GetById(Guid id)
    {
        var coach = _repository.GetById(id);
        if (coach == null) return NotFound();

        return Ok(CoachMapper.ToDto(coach));    //fix
    }

    [HttpGet]
    public ActionResult<IEnumerable<CoachDto>> GetAll()
    {
        var coaches = _repository.GetAll();

        return Ok(coaches.Select(CoachMapper.ToDto));
    }

    [HttpPost]
    public ActionResult Add([FromBody] CoachDto dto)
    {
        var coach = CoachMapper.ToDomain(dto);
        _repository.Add(coach);     // fix

        return CreatedAtAction(nameof(GetById), new { id = coach.Id }, CoachMapper.ToDto(coach));
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(Guid id)
    {
        if (!_repository.Remove(id))
            return NotFound();

        return NoContent();
    }
}