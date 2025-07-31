using HorsesForCourses.Core;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("courses")]
public class CourseController : ControllerBase
{
    private readonly InMemoryCourseRepository _repository;
    private readonly InMemoryCoachRepository _coachRepository;
    private readonly CourseScheduler _courseScheduler;

    public CourseController(InMemoryCourseRepository repository, InMemoryCoachRepository coachRepository, CourseScheduler courseScheduler)
    {
        _repository = repository;
        _coachRepository = coachRepository;
        _courseScheduler = courseScheduler;
    }

    // POST course
    [HttpPost]
    public ActionResult<CourseDto> Create([FromBody] CreateCourseDto dto)
    {
        var period = new Period(dto.startDate.Date, dto.endDate.Date);
        var course = new Course(dto.Title, period);

        _repository.Add(course);

        return CreatedAtAction(nameof(GetById), new { id = course.Id }, CourseMapper.ToDto(course));
    }

    // POST skill in course
    [HttpPost("{id}/skills")]
    public ActionResult UpdateSkills(int id, [FromBody] UpdateCourseSkillsDto dto)
    {
        var course = _repository.GetById(id);
        if (course == null) return NotFound();

        course.UpdateRequiredSkills(dto.Skills);

        return NoContent();
    }

    // POST TimeSlot
    [HttpPost("{id}/timeslots")]
    public ActionResult UpdateTimeSlots(int id, [FromBody] UpdateCourseScheduleDto dto)
    {
        var course = _repository.GetById(id);

        if (course == null)
            return NotFound();

        var (success, error) = _courseScheduler.UpdateSchedule(course, dto.TimeSlots);

        if (!success)
            return BadRequest(error);

        return NoContent();
    }

    // Confrim Course
    [HttpPost("{id}/confirm")]
    public ActionResult Confirm(int id)
    {
        var course = _repository.GetById(id);

        if (course == null)
            return NotFound();

        course.Confirm();

        return NoContent();
    }

    // Asing coach
    [HttpPost("{id}/assign-coach")]
    public ActionResult AssignCoach(int id, [FromBody] AssignCoachDto dto)
    {
        var course = _repository.GetById(id);

        if (course == null)
            return NotFound("There is no course");

        var coach = _coachRepository.GetById(dto.CoachId);

        if (coach == null)
            return NotFound("Coach not found.");

        course.AssignCoach(coach);

        return NoContent();
    }

    // GET all courses
    [HttpGet]
    public ActionResult<List<CourseDto>> GetAll()
    {
        var courses = _repository.GetAll();
        var dtoList = courses.Select(CourseMapper.ToDto).ToList();
        return Ok(dtoList);
    }

    // GET by Id
    [HttpGet("{id}")]
    public ActionResult<CourseDto> GetById(int id)
    {
        var course = _repository.GetById(id);

        if (course == null)
            return NotFound();

        return Ok(CourseMapper.ToDto(course));
    }





}

