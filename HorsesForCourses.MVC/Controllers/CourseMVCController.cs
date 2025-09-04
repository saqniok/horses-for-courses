using HorsesForCourses.Core;
using HorsesForCourses.Service;
using HorsesForCourses.Service.DTOs;
using HorsesForCourses.Service.Queries;
using HorsesForCourses.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HorsesForCourses.MVC.Controllers
{
    public class CreateCourseRequest
    {
        public string Title { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }

    public class CourseMVCController : Controller
    {
        private readonly IGetCourseSummariesQuery _getCoursesQuery;
        private readonly ICourseService _courseService;

        public CourseMVCController(IGetCourseSummariesQuery getCoursesQuery, ICourseService courseService)
        {
            _getCoursesQuery = getCoursesQuery;
            _courseService = courseService;
        }

        // GET: Courses
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            return View(await _getCoursesQuery.All(new PageRequest(page, pageSize)));
        }

        // GET: Course/Details/5
        [HttpGet("Course/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // GET: Course/Create
        [HttpGet("Course/Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Course/Create
        [HttpPost("Course/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,StartDate,EndDate")] CreateCourseRequest request)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var timeDay = new TimeDay(request.StartDate, request.EndDate);
                    var course = new Course(request.Title, timeDay);
                    await _courseService.CreateAsync(course);
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(request);
        }

        // GET: Course/Edit/5
        [HttpGet("Course/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _courseService.GetDtoByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // POST: Course/Edit/5
        [HttpPost("Course/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Title,StartDate,EndDate")] CreateCourseRequest request)
        {
            var courseToUpdate = await _courseService.GetByIdAsync(id);
            if (courseToUpdate == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    courseToUpdate.UpdateTitle(request.Title);
                    await _courseService.UpdateAsync(courseToUpdate);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Unable to save changes. " + ex.Message);
                }
            }
            return View(request);
        }

        // GET: Course/Delete/5
        [HttpGet("Course/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // POST: Course/Delete/5
        [HttpPost("Course/Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _courseService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}