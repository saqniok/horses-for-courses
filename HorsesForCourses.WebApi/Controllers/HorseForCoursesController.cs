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
        return Ok(coach);
    }

    [HttpGet]
    public ActionResult<IEnumerable<Coach>> GetAll()
    {
        var coaches = _repository.GetAll();
        return Ok(coaches);
    }

    [HttpPost]
    public ActionResult Add([FromBody] Coach coach)
    {
        _repository.Add(coach);
        return CreatedAtAction(nameof(GetById), new { id = coach.Id }, coach);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(Guid id)
    {
        if (!_repository.Remove(id))
            return NotFound();

        return NoContent();
    }
}
