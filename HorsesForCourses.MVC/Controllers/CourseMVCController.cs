using HorsesForCourses.Core;
using HorsesForCourses.Service.DTOs;
using HorsesForCourses.Service.Queries;
using HorsesForCourses.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HorsesForCourses.MVC.Controllers
{
    [Authorize]
    public class CourseMVCController : Controller
    {
        private readonly IGetCourseSummariesQuery _getCoursesQuery;
        private readonly ICourseService _courseService;
        private readonly ICoachService _coachService;

        public CourseMVCController(IGetCourseSummariesQuery getCoursesQuery, ICourseService courseService, ICoachService coachService)
        {
            _getCoursesQuery = getCoursesQuery;
            _courseService = courseService;
            _coachService = coachService;
        }

        // GET: Courses
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            return View(await _getCoursesQuery.All(new PageRequest(page, pageSize)));
        }

        // GET: Course/Details/5
        [HttpGet("Course/Details/{id}")]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Details(int id)
        {
            var course = await _courseService.GetDtoByIdAsync(id);
            if (course == null)
                return NotFound();

            return View(course);
        }

        // GET: Course/Create
        [HttpGet("Course/Create")]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Course/Create
        [HttpPost("Course/Create")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Title,startDate,endDate")] CreateCourseRequest request)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var timeDay = new TimeDay(request.startDate, request.endDate);
                    var course = new Course(request.Title, timeDay);
                    await _courseService.CreateAsync(course);
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(request);
        }

        // GET: Course/Edit/5
        [HttpGet("Course/Edit/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _courseService.GetDtoByIdAsync(id);
            if (course == null)
                return NotFound();

            return View(course);
        }

        // POST: Course/Edit/5
        [HttpPost("Course/Edit/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Title")] CourseDto courseDto)
        {
            var courseToUpdate = await _courseService.GetByIdAsync(id);
            if (courseToUpdate == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    courseToUpdate.UpdateTitle(courseDto.Title);
                    await _courseService.UpdateAsync(courseToUpdate);
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(courseDto);
        }

        // POST: Course/AddSkill/5
        [HttpPost("Course/AddSkill/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddSkill(int id, [FromForm] string skill)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null)
                return NotFound();

            if (!string.IsNullOrWhiteSpace(skill))
            {
                try
                {
                    course.AddRequiredSkill(skill);
                    await _courseService.UpdateAsync(course);
                }
                catch (InvalidOperationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            return RedirectToAction(nameof(Edit), new { id = id });
        }

        // POST: Course/RemoveSkill/5
        [HttpPost("Course/RemoveSkill/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveSkill(int id, [FromForm] string skill)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null)
                return NotFound();

            if (!string.IsNullOrWhiteSpace(skill))
            {
                try
                {
                    course.RemoveRequiredSkill(skill);
                    await _courseService.UpdateAsync(course);
                }
                catch (InvalidOperationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            return RedirectToAction(nameof(Edit), new { id = id });
        }

        // POST: Course/AddTimeSlot/5
        [HttpPost("Course/AddTimeSlot/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddTimeSlot(int id, [FromForm] WeekDay day, [FromForm] int startTime, [FromForm] int endTime)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null)
                return NotFound();

            if (startTime < endTime && startTime >= 0 && endTime <= 24)
            {
                try
                {
                    var timeSlot = new TimeSlot(day, startTime, endTime);
                    course.AddTimeSlot(timeSlot);
                    await _courseService.UpdateAsync(course);
                }
                catch (InvalidOperationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            return RedirectToAction(nameof(Edit), new { id = id });
        }

        // POST: Course/RemoveTimeSlot/5
        [HttpPost("Course/RemoveTimeSlot/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveTimeSlot(int id, [FromForm] WeekDay day, [FromForm] int startTime, [FromForm] int endTime)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null)
                return NotFound();

            if (!course.IsConfirmed)
            {
                try
                {
                    // Find and remove the time slot
                    var timeSlotToRemove = course.Schedule.FirstOrDefault(ts =>
                        ts.Day == day && ts.Start == startTime && ts.End == endTime);

                    if (timeSlotToRemove != null)
                    {
                        // Note: This is a simplified approach. In a real implementation,
                        // you might want to add a RemoveTimeSlot method to the Course class
                        // For now, we'll clear and re-add all except the one to remove
                        var schedule = course.Schedule.ToList();
                        schedule.Remove(timeSlotToRemove);
                        course.UpdateTimeSlot(schedule);
                        await _courseService.UpdateAsync(course);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            return RedirectToAction(nameof(Edit), new { id = id });
        }

        // POST: Course/Confirm/5
        [HttpPost("Course/Confirm/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Confirm(int id)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null)
                return NotFound();

            if (!course.IsConfirmed)
            {
                try
                {
                    course.Confirm();
                    await _courseService.UpdateAsync(course);
                }
                catch (InvalidOperationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            return RedirectToAction(nameof(Edit), new { id = id });
        }

        // GET: Course/AssignCoach/5
        [HttpGet("Course/AssignCoach/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignCoach(int id)
        {
            var course = await _courseService.GetDtoByIdAsync(id);
            if (course == null)
                return NotFound();

            // Get all coaches for assignment
            var coaches = await _coachService.GetAllAsync();
            ViewBag.Coaches = coaches;

            return View(course);
        }

        // POST: Course/AssignCoach/5
        [HttpPost("Course/AssignCoach/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignCoachPost(int id, [FromForm] int coachId)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null)
                return NotFound();

            var coach = await _coachService.GetByIdAsync(coachId);
            if (coach == null)
            {
                TempData["Error"] = "Selected coach not found.";
                return RedirectToAction(nameof(AssignCoach), new { id = id });
            }

            try
            {
                course.AssignCoach(coach);
                await _courseService.UpdateAsync(course);
            }
            catch (InvalidOperationException ex)
            {
                    TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(AssignCoach), new { id = id });
            }

            return RedirectToAction(nameof(Edit), new { id = id });
        }

        // GET: Course/Delete/5
        [HttpGet("Course/Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null)
                return NotFound();

            return View(course);
        }

        // POST: Course/Delete/5
        [HttpPost("Course/Delete/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _courseService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
