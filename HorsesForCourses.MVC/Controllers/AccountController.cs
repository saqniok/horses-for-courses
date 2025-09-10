

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Service.Interfaces;
using HorsesForCourses.MVC.Models.ViewModels;
using HorsesForCourses.Core;

namespace HorsesForCourses.MVC.Controllers;

public class AccountController : Controller
{
    private readonly IUserService _userService;
    private readonly IPasswordHasher _passwordHasher;

    public AccountController(IUserService userService, IPasswordHasher passwordHasher)
    {
        _userService = userService;
        _passwordHasher = passwordHasher;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userService.GetByEmailAsync(model.Email);

            if (user != null && _passwordHasher.Verify(model.Password, user.PasswordHash))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Email)
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity));
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }
        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterAccountViewModel model)
    {
        if (ModelState.IsValid)
        {
            var existingUser = await _userService.GetByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "User with this email already exists.");
                return View(model);
            }

            var hashedPassword = _passwordHasher.Hash(model.Pass);
            var newUser = new User(model.Name, model.Email, hashedPassword);

            await _userService.CreateAsync(newUser);

            // Automatically log in the user after registration
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, newUser.Id.ToString()),
                new Claim(ClaimTypes.Name, newUser.Email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("Cookies");
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> DownloadUserData()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Unauthorized();
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return BadRequest("User ID not found.");
        }

        var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
        var user = await _userService.GetByEmailAsync(userEmail);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        var userData = $"Name: {user.Name}\nEmail: {user.Email}";
        var fileName = $"user_data_{user.Id}.txt";
        var contentType = "text/plain";

        return File(System.Text.Encoding.UTF8.GetBytes(userData), contentType, fileName);
    }

    [HttpGet]
    public IActionResult DeleteAccountConfirmation()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login");
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAccount()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Unauthorized();
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return BadRequest("User ID not found.");
        }

        await _userService.DeleteAsync(userId);
        await HttpContext.SignOutAsync("Cookies");

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public IActionResult ExternalLogin(string provider, string returnUrl = null)
    {
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
        var properties = _userService.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return new ChallengeResult(provider, properties);
    }

    [HttpGet]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
    {
        if (remoteError != null)
        {
            ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
            return View(nameof(Login));
        }

        var info = await HttpContext.AuthenticateAsync("External");
        if (info?.Principal == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var emailClaim = info.Principal.FindFirst(ClaimTypes.Email);
        if (emailClaim == null)
        {
            ModelState.AddModelError(string.Empty, "External authentication provider did not provide an email address.");
            return View(nameof(Login));
        }

        var email = emailClaim.Value;
        var user = await _userService.GetByEmailAsync(email);

        if (user == null)
        {
            // User does not exist, create a new one
            var nameClaim = info.Principal.FindFirst(ClaimTypes.Name);
            var name = nameClaim?.Value ?? email; // Use email as name if name claim is not available

            var newUser = new User(name, email, _passwordHasher.Hash(Guid.NewGuid().ToString())); // Generate a random password for social login users
            await _userService.CreateAsync(newUser);
            user = newUser;
        }

        // Sign in the user with application cookies
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email)
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
        await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity));

        await HttpContext.SignOutAsync("External"); // Sign out from external cookie

        return RedirectToLocal(returnUrl);
    }

    private IActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        else
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
