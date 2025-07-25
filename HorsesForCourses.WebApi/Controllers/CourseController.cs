using HorsesForCourses.Core;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("courses")]
public class CourseController : ControllerBase
{
    private readonly InMemoryCourseRepository _repository;
    private readonly InMemoryCoachRepository _coachRepository; // Для coach

    public CourseController(InMemoryCourseRepository repository, InMemoryCoachRepository coachRepository)
    {
        _repository = repository;
        _coachRepository = coachRepository;
    }

    [HttpPost]
    public ActionResult<CourseDto> Create([FromBody] CreateCourseDto dto)
    {
        var period = new Period(dto.PeriodStart, dto.PeriodEnd);
        var course = new Course(dto.Title, period);

        _repository.Add(course);

        return CreatedAtAction(nameof(GetById), new { id = course.Id }, CourseMapper.ToDto(course));
    }


    // GET /courses/{id}
    [HttpGet("{id}")]
    public ActionResult<CourseDto> GetById(Guid id)
    {
        var course = _repository.GetById(id);
        if (course == null) return NotFound();

        return Ok(CourseMapper.ToDto(course));
    }

    // GET /courses
    [HttpGet]
    public ActionResult<List<CourseDto>> GetAll()
    {
        var courses = _repository.GetAll();
        var dtoList = courses.Select(CourseMapper.ToDto).ToList();
        return Ok(dtoList);
    }

    // POST /courses/{id}/skills - обновить навыки
    [HttpPost("{id}/skills")]
    public ActionResult UpdateSkills(Guid id, [FromBody] UpdateCourseSkillsDto dto)
    {
        var course = _repository.GetById(id);
        if (course == null) return NotFound();

        // Очистим текущие навыки
        foreach (var skill in course.RequiredSkills.ToList())
            course.RemoveRequiredSkill(skill);

        // Добавим из запроса
        foreach (var skill in dto.Skills)
            course.AddRequiredSkill(skill);

        return NoContent();
    }

    // POST /courses/{id}/timeslots - обновить расписание
    [HttpPost("{id}/timeslots")]
    public ActionResult UpdateTimeSlots(Guid id, [FromBody] UpdateCourseScheduleDto dto)
    {
        var course = _repository.GetById(id);
        if (course == null) return NotFound();

        // Очистим текущее расписание
        foreach (var ts in course.Schedule.ToList())
            course.RemoveTimeSlot(ts);

        foreach (var tsDto in dto.TimeSlots)
        {
            if (!Enum.IsDefined(typeof(WeekDay), tsDto.Day))
                return BadRequest($"Invalid day value: {tsDto.Day}");

            var timeSlot = new TimeSlot(tsDto.Day, tsDto.Start, tsDto.End);
            course.AddTimeSlot(timeSlot);
        }

        return NoContent();
    }

    // POST /courses/{id}/confirm - подтвердить курс
    [HttpPost("{id}/confirm")]
    public ActionResult Confirm(Guid id)
    {
        var course = _repository.GetById(id);
        if (course == null) return NotFound();

        try
        {
            course.Confirm();
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }

        return NoContent();
    }

    // POST /courses/{id}/assign-coach - назначить тренера
    [HttpPost("{id}/assign-coach")]
    public ActionResult AssignCoach(Guid id, [FromBody] AssignCoachDto dto)
    {
        var course = _repository.GetById(id);
        if (course == null) return NotFound();

        var coach = _coachRepository.GetById(dto.CoachId);
        if (coach == null) return NotFound("Coach not found.");

        try
        {
            course.AssignCoach(coach);
            coach.AssignCourse(course);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }

        return NoContent();
    }
}

