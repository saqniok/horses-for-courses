using HorsesForCourses.Core;
using HorsesForCourses.Service;
using HorsesForCourses.Service.DTOs;
using HorsesForCourses.Service.Queries;
using HorsesForCourses.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HorsesForCourses.MVC.Controllers
{
    [Authorize]
    public class CoachMVCController : Controller
    {
        private readonly IGetCoachSummariesQuery _getCoachSummaries;
        private readonly ICoachService _coachService;

        public CoachMVCController(IGetCoachSummariesQuery getCoachSummaries, ICoachService coachService)
        {
            _getCoachSummaries = getCoachSummaries;
            _coachService = coachService;
        }

        // GET: Coaches
        [HttpGet]
        [AllowAnonymous]
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
                return NotFound();

            return View(coach);
        }

        // GET: Coaches/Create
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Coaches/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Name,Email")] CreateCoachRequest request)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var coach = new Coach(request.Name, request.Email);
                    await _coachService.CreateAsync(coach);
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(request);
        }

        // GET: Coaches/Edit/5
        [HttpGet("Coaches/Edit/{id}")]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Edit(int id)
        {
            var coach = await _coachService.GetDtoByIdAsync(id);
            
            if (coach == null)
                return NotFound();

            // Pass CoachDetailsDto directly to the view
            return View(coach);
        }

        // POST: Coaches/Edit/5
        [HttpPost("Coaches/Edit/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Email")] CreateCoachRequest request)
        {
            if (id == 0)
                return NotFound();


            if (ModelState.IsValid)
            {
                var coach = await _coachService.GetByIdAsync(id);
                if (coach == null)
                    return NotFound();

                try
                {
                    coach.UpdateDetails(request.Name, request.Email);
                    await _coachService.UpdateAsync(coach);
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            // If model state is not valid, re-fetch coach details to display skills correctly
            var coachDetails = await _coachService.GetDtoByIdAsync(id);
            return View(coachDetails);
        }

        // GET: Coaches/Delete/5
        [HttpGet("Coaches/Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var coach = await _coachService.GetDtoByIdAsync(id);
            if (coach == null)
                return NotFound();

            return View(coach);
        }

        // POST: Coaches/Delete/5
        [HttpPost("Coaches/Delete/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coach = await _coachService.GetByIdAsync(id);
            if (coach == null)
                return NotFound();

            await _coachService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: Coaches/AddSkill/5
        [HttpPost("Coaches/AddSkill/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> AddSkill(int id, [FromForm] string skill)
        {
            var coach = await _coachService.GetByIdAsync(id);
            if (coach == null)
                return NotFound();

            if (User.IsInRole(UserRole.Coach.ToString()) && User.FindFirst(ClaimTypes.NameIdentifier)?.Value != id.ToString())
                return Forbid();

            if (!string.IsNullOrWhiteSpace(skill))
            {
                try
                {
                    coach.AddSkill(skill);
                    await _coachService.UpdateAsync(coach);
                }
                catch (ArgumentException ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            return RedirectToAction(nameof(Edit), new { id = id }); // Redirect to Edit
        }

        // POST: Coaches/RemoveSkill/5
        [HttpPost("Coaches/RemoveSkill/{id}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Coach")]
        public async Task<IActionResult> RemoveSkill(int id, [FromForm] string skill)
        {
            if (User.IsInRole(UserRole.Coach.ToString()) && User.FindFirst(ClaimTypes.NameIdentifier)?.Value != id.ToString())
            {
                return Forbid();
            }

            try
            {
                await _coachService.RemoveSkillAsync(id, skill);
            }
            catch (ArgumentException ex)
            {
                TempData["Error"] = ex.Message;
            }
            
            return RedirectToAction(nameof(Edit), new { id = id }); // Redirect to Edit
        }
    }
}
