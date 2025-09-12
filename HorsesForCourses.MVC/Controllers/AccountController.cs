

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Service.Interfaces;
using HorsesForCourses.MVC.Models.ViewModels;
using System.Text.Json;
using HorsesForCourses.Core;
using HorsesForCourses.Service.DTOs;

namespace HorsesForCourses.MVC.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService; // Keep for DownloadUserData

    public AccountController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*
        Когда сервер отдает страницу с формой, в ней с помощью `@Html.AntiForgeryToken()` 
        (или asp-antiforgery="true") вставляется скрытое поле __RequestVerificationToken.

        Дополнительно этот же токен сохраняется в cookie пользователя
    */
    
    // IActionResult — это interface, который представляет результат действия контроллера (например, возврат View, RedirectToAction, JsonResult и т.д.)
    public async Task<IActionResult> Login(LoginViewModel model)
    /*
        Имя метода — Login. Он принимает один параметр model типа LoginViewModel, который 
        обычно содержит данные, отправленные из формы входа (например, Email и Password). 
        Model binding в ASP.NET Core автоматически заполняет этот объект данными из запроса
    */
    {
        if (ModelState.IsValid)
        /*
            Это условие проверяет, действительны ли данные, отправленные в LoginViewModel, 
            согласно правилам data annotations (например, [Required], [EmailAddress]) в модели. 
            Если какие-то поля не прошли валидацию, это условие будет false
        */
        {
            var loginDto = new LoginDto
            {
                Email = model.Email,
                Password = model.Password
            };

            var (claimsPrincipal, errors) = await _authService.LoginAsync(loginDto);

            if (claimsPrincipal != null)
                return RedirectToAction("Index", "Home");
                /*
                    После успешного входа метод возвращает RedirectToAction, 
                    который перенаправляет пользователя на страницу с именем Index в контроллере Home
                */


            foreach (var error in errors)
                ModelState.AddModelError(error.Key, error.Value);
            /*
                Если проверка if (сравнение пароля) не прошла, 
                эта строка добавляет сообщение об ошибке в ModelState. 
                Это сообщение можно отобразить на странице входа, 
                используя Validation Summary или validation tags в Razor View
            */
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
        /*
            Это условие проверяет, прошли ли данные из model первичную валидацию, 
            определённую через data annotations в RegisterAccountViewModel. 
            Например, оно проверит, что email имеет правильный формат, а пароль не пуст        
        */
        {
            var registerDto = new RegisterAccountDto
            {
                Name = model.Name,
                Email = model.Email,
                Password = model.Pass,
                ConfirmPassword = model.ConfirmPass, // Assuming RegisterAccountViewModel has ConfirmPass
                IsAdmin = model.IsAdmin,
                IsCoach = model.IsCoach
            };
            /*
                Создаётся (DTO), который служит для передачи данных 
                от контроллера к сервису. Это важный шаг, так как он отделяет логику 
                представления (ViewModel) от бизнес-логики (DTO). 
                `ConfirmPassword` из `ViewModel` также копируется в DTO, 
                что подразумевает, что логика проверки совпадения паролей 
                находится внутри _authService
            */

            var (claimsPrincipal, errors) = await _authService.RegisterAsync(registerDto);

            if (claimsPrincipal != null)
                return RedirectToAction("Index", "Home");

            foreach (var error in errors)
                ModelState.AddModelError(error.Key, error.Value);
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _authService.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> DownloadUserData()
    {
        if (!User.Identity!.IsAuthenticated) // Corrected condition
            return Unauthorized();

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return BadRequest("User ID not found.");

        var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
        if (string.IsNullOrEmpty(userEmail))
            return BadRequest("User email not found.");

        var user = await _authService.GetUserByEmailAsync(userEmail); // Using AuthService
        if (user == null)
            return NotFound("User not found.");

        // Create an anonymous object with the desired user data, excluding sensitive information like PasswordHash
        var userData = new
        {
            user!.Id,
            user.Name,
            user.Email,
            Role = user.Role.ToString()
        };

        var jsonUserData = JsonSerializer.Serialize(userData, new JsonSerializerOptions { WriteIndented = true });
        var fileName = $"user_data_{user.Id}.json";
        var contentType = "application/json";

        return File(System.Text.Encoding.UTF8.GetBytes(jsonUserData), contentType, fileName);
    }

    [HttpGet]
    public IActionResult DeleteAccountConfirmation()
    {
        if (!User.Identity!.IsAuthenticated)
            return RedirectToAction("Login");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAccount()
    {
        if (!User.Identity!.IsAuthenticated)
            return Unauthorized();

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return BadRequest("User ID not found.");

        await _authService.DeleteAccountAsync(userId);
        await _authService.SignOutAsync();

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ExternalLogin(string provider, string? returnUrl = null)
    {
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
        var properties = _authService.ConfigureExternalAuthenticationProperties(provider, redirectUrl!);
        return new ChallengeResult(provider, properties);
    }

    [HttpGet]
    public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
    {
        var (claimsPrincipal, errors) = await _authService.ExternalLoginCallbackAsync(returnUrl, remoteError);

        if (claimsPrincipal != null)
            return RedirectToLocal(returnUrl);

        foreach (var error in errors)
            ModelState.AddModelError(error.Key, error.Value);

        return View(nameof(Login));
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }
}
