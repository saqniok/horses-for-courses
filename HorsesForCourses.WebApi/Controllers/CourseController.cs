using HorsesForCourses.Core;
using Microsoft.AspNetCore.Mvc;
using QuickPulse.Show;

[ApiController]
[Route("courses")]
public class CourseController : ControllerBase
{
    private readonly ICourseRepository _repository;
    private readonly ICoachRepository _coachRepository;
    private readonly CourseScheduler _courseScheduler;

    // add EFCourseRepository 
    public CourseController(
        // add it here first and switch step by step
        ICourseRepository repository,
        ICoachRepository coachRepository,
        CourseScheduler courseScheduler)
    {
        _repository = repository;
        _coachRepository = coachRepository;
        _courseScheduler = courseScheduler;
    }

    // POST course
    [HttpPost]
    public ActionResult<CourseDto> Create([FromBody] CreateCourseDto dto) // needs to be Task (async)
    {
        var start = new DateOnly(2025, 8, 1);
        var end = new DateOnly(2025, 8, 31);
        var period = new Period(start, end);
        var course = new Course(dto.Title, period);

        _repository.Add(course); // switch this to new EF Repo
        
        _repository.SaveChanges();

        return CreatedAtAction(nameof(GetById), new { id = course.Id }, CourseMapper.ToDto(course));
    }

    // POST skill in course
    [HttpPost("{id}/skills")]
    public ActionResult UpdateSkills(int id, [FromBody] UpdateCourseSkillsDto dto)
    {
        var course = _repository.GetById(id);

        if (course == null)
            return NotFound();

        course.UpdateRequiredSkills(dto.Skills);

        _repository.SaveChanges();

        return NoContent();
    }

    // POST TimeSlot
    [HttpPost("{id}/timeslots")]
    public ActionResult UpdateTimeSlots(int id, [FromBody] UpdateCourseScheduleDto dto)
    {
        var course = _repository.GetById(id); // take this second

        if (course == null)
            return NotFound();

        var timeSlots = dto.TimeSlots.ToDomain();

        var (success, error) = _courseScheduler.UpdateSchedule(course, timeSlots);

        if (!success)
            return BadRequest(error);

        _repository.SaveChanges();

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
        _repository.SaveChanges();

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
        _repository.SaveChanges();

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

/**
    Это контроллер ASP.NET Core Web API для управления курсами. Он отвечает за обработку HTTP-запросов, связанных с курсами, и взаимодействие с бизнес-логикой.

    - Зависимости: Контроллер внедряет зависимости InMemoryCourseRepository, InMemoryCoachRepository и CourseScheduler. Это указывает на использование In-Memory репозиториев для хранения данных, что, как я упоминал ранее, хорошо для тестирования и быстрой разработки, но в реальном приложении будет заменено на постоянное хранилище (например, через EFCourseRepository).
    - Маршрутизация: Атрибуты [ApiController] и [Route("courses")] указывают, что это API-контроллер и что все его методы будут доступны по базовому маршруту /courses.
    - HTTP-методы (Endpoints):
        - Create (POST /courses): Создает новый курс. Принимает CreateCourseDto и использует CourseMapper для преобразования в доменную модель Course.
        - UpdateSkills (POST /courses/{id}/skills): Обновляет необходимые навыки для курса.
        - UpdateTimeSlots (POST /courses/{id}/timeslots): Обновляет расписание курса. Использует CourseScheduler для проверки и обновления расписания, что, вероятно, включает проверку на пересечения.
        - Confirm (POST /courses/{id}/confirm): Подтверждает курс.
        - AssignCoach (POST /courses/{id}/assign-coach): Назначает тренера курсу.
        - GetAll (GET /courses): Получает список всех курсов.
        - GetById (GET /courses/{id}): Получает информацию о конкретном курсе по его ID.
        - В целом, CourseController действует как шлюз между внешним миром (HTTP-запросами) и внутренней бизнес-логикой, используя DTO для удобного обмена данными и репозитории для доступа к данным.

    Надеюсь, это подробное объяснение поможет вам понять структуру и назначение файлов в этом проекте!
*/

