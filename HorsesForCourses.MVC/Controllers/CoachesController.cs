using HorsesForCourses.Core;
using HorsesForCourses.Service;
using HorsesForCourses.Service.DTOs;
using HorsesForCourses.Service.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HorsesForCourses.MVC.Controllers
{
    public class CoachesController : Controller
    {
        private readonly IGetCoachSummariesQuery _getCoachSummaries;
        private readonly ICoachService _coachService;

        public CoachesController(IGetCoachSummariesQuery getCoachSummaries, ICoachService coachService)
        {
            _getCoachSummaries = getCoachSummaries;
            _coachService = coachService;
        }

        // GET: Coaches
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
        {
            var coaches = await _getCoachSummaries.All(new PageRequest(page, pageSize));
            return View(coaches);
        }

        // GET: Coaches/Details/5
        [HttpGet("Coaches/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var coach = await _coachService.GetDtoByIdAsync(id);
            if (coach == null)
            {
                return NotFound();
            }
            return View(coach);
        }

        // GET: Coaches/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Coaches/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Email")] CreateCoachRequest request)
        {
            if (ModelState.IsValid)
            {
                var coach = new Coach(request.Name, request.Email);
                await _coachService.CreateAsync(coach);
                return RedirectToAction(nameof(Index));
            }
            return View(request);
        }

        // GET: Coaches/Edit/5
        [HttpGet("Coaches/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var coach = await _coachService.GetDtoByIdAsync(id);
            if (coach == null)
            {
                return NotFound();
            }
            // Pass CoachDetailsDto directly to the view
            return View(coach);
        }

        // POST: Coaches/Edit/5
        [HttpPost("Coaches/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Email")] CreateCoachRequest request)
        {
            if (id == 0) 
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var coach = await _coachService.GetByIdAsync(id);
                if (coach == null)
                {
                    return NotFound();
                }
                coach.UpdateDetails(request.Name, request.Email);
                await _coachService.UpdateAsync(coach);
                return RedirectToAction(nameof(Index));
            }
            // If model state is not valid, re-fetch coach details to display skills correctly
            var coachDetails = await _coachService.GetDtoByIdAsync(id);
            return View(coachDetails);
        }

        // GET: Coaches/Delete/5
        [HttpGet("Coaches/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var coach = await _coachService.GetDtoByIdAsync(id);
            if (coach == null)
            {
                return NotFound();
            }
            return View(coach);
        }

        // POST: Coaches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coach = await _coachService.GetByIdAsync(id);
            if (coach == null)
            {
                return NotFound();
            }
            await _coachService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: Coaches/AddSkill/5
        [HttpPost("Coaches/AddSkill/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSkill(int id, [FromForm] string skill)
        {
            var coach = await _coachService.GetByIdAsync(id);
            if (coach == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(skill))
            {
                coach.AddSkill(skill);
                await _coachService.UpdateAsync(coach);
            }

            return RedirectToAction(nameof(Edit), new { id = id }); // Redirect to Edit
        }

        // POST: Coaches/RemoveSkill/5
        [HttpPost("Coaches/RemoveSkill/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveSkill(int id, [FromForm] string skill)
        {
            try
            {
                await _coachService.RemoveSkillAsync(id, skill);
            }
            catch (InvalidOperationException)
            {
                // Handle case where skill doesn't exist or other errors
            }
            return RedirectToAction(nameof(Edit), new { id = id }); // Redirect to Edit
        }
    }
}
