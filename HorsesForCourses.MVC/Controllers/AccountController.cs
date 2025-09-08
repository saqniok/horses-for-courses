

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace HorsesForCourses.MVC.Controllers;

public class AccountController : Controller
{



    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // AccountController.cs
    [HttpPost]
    public async Task<IActionResult> Login(string email)
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, email) };
        var id = new ClaimsIdentity(claims, "Cookies");
        await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(id));
        return Redirect("/");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("Cookies");
        return Redirect("/");
    }
}