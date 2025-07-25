using HorsesForCourses.Core;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("courses")]
public class CourseController : ControllerBase
{
    private readonly InMemoryCourseRepository _repository;

    public CourseController(InMemoryCourseRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CourseDto>> GetAll()
    {
        var courses = _repository.GetAll()
            .Select(CourseMapper.ToDto)
            .ToList();
        return Ok(courses);
    }

    [HttpGet("{id}")]
    public ActionResult<CourseDto> GetById(Guid id)
    {
        var course = _repository.GetById(id);
        if (course == null) return NotFound();

        return Ok(CourseMapper.ToDto(course));
    }

    [HttpPost]
    public ActionResult<CourseDto> Create(CreateCourseDto dto)
    {
        var period = new Period(dto.PeriodStart, dto.PeriodEnd);
        var course = new Course(dto.Title, period);

        foreach (var skill in dto.RequiredSkills)
            course.AddRequiredSkill(skill);

        foreach (var ts in dto.Schedule)
        {
            var timeSlot = new TimeSlot(ts.Day, ts.Start, ts.End);
            course.AddTimeSlot(timeSlot);
        }



        // Можно сразу подтверждать, если расписание есть
        if (course.Schedule.Any())
            course.Confirm();

        _repository.Add(course);

        return CreatedAtAction(nameof(GetById), new { id = course.Id }, CourseMapper.ToDto(course));
    }

}
