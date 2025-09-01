using HorsesForCourses.Core;
using HorsesForCourses.Service.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HorsesForCourses.MVC.Controllers
{
    public class CoachesController(IGetCoachSummariesQuery getCoachSummaries) : Controller
    {
        private readonly IGetCoachSummariesQuery getCoachSummaries = getCoachSummaries;

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
            => View(await getCoachSummaries.All(new PageRequest(page, pageSize)));
    }
}