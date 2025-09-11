

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Service.Interfaces;
using HorsesForCourses.MVC.Models.ViewModels;
using System.Text.Json;
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
            var user = await _userService.GetByEmailAsync(model.Email);
            /*
                _userService.GetByEmailAsync(model.Email) ищет пользователя в базе данных 
                по предоставленному email. await приостанавливает выполнение метода, 
                пока асинхронная операция не завершится, не блокируя при этом поток
            */

            if (user != null && _passwordHasher.Verify(model.Password, user.PasswordHash))
            {
                /*
                    _passwordHasher.Verify(...)
                    Проверяет, соответствует ли предоставленный пароль (model.Password)
                    хешированному паролю пользователя (user.PasswordHash), 
                    хранящемуся в базе данных. 
                    Использование password hashing критически важно для безопасности
                */
                var claims = new List<Claim>
                /*
                    Если аутентификация прошла успешно, 
                    создается список claims. Claims — это утверждения (statement) 
                    о пользователе, которые используются для идентификации и авторизации
                */
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    // ClaimTypes.NameIdentifier — это стандартный тип claim для уникального идентификатора пользователя. 
                    // user.Id.ToString() преобразует ID пользователя в строку
                    new Claim(ClaimTypes.Name, user.Email)
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                /*
                    Создается ClaimsIdentity, который представляет личность пользователя. 
                    Он содержит список claims и указывает, какой тип аутентификации используется 
                    ("Cookies"). Это необходимо для создания authentication cookie
                */

                await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity));
                /*
                    Метод принимает ClaimsPrincipal (который, в свою очередь, содержит ClaimsIdentity)
                    и схему аутентификации ("Cookies"). ASP.NET Core создает 
                    authentication cookie и отправляет его в браузер пользователя. 
                    Теперь пользователь считается аутентифицированным
                */

                return RedirectToAction("Index", "Home");
                /*
                    После успешного входа метод возвращает RedirectToAction, 
                    который перенаправляет пользователя на страницу с именем Index в контроллере Home
                */
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
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
        {
            var existingUser = await _userService.GetByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "User with this email already exists.");
                // Добавляет ошибку в ModelState, привязывая её к полю "Email".

                return View(model);
            }

            var hashedPassword = _passwordHasher.Hash(model.Pass);
            /*
                Хеширует пароль, введенный пользователем. 
                Это критически важно для безопасности, так как 
                в базу данных нельзя хранить пароли в открытом виде
            */
            UserRole selectedRole = UserRole.User; // Default

            if (model.IsAdmin)
            {
                selectedRole = UserRole.Admin;
            }
            else if (model.IsCoach)
            {
                selectedRole = UserRole.Coach;
            }
            // If neither Admin nor Coach is selected, it defaults to User (which is already set)

            var newUser = new User(model.Name, model.Email, hashedPassword, selectedRole);

            await _userService.CreateAsync(newUser);

            // Automatically log in the user after registration
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, newUser.Id.ToString()),
                new Claim(ClaimTypes.Name, newUser.Email),
                new Claim(ClaimTypes.Role, newUser.Role.ToString())
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
        if (!User.Identity!.IsAuthenticated)
        {
            return Unauthorized();
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return BadRequest("User ID not found.");
        }

        var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
        if (string.IsNullOrEmpty(userEmail))
        {
            return BadRequest("User email not found.");
        }

        var user = await _userService.GetByEmailAsync(userEmail);

        if (user == null)
        {
            return NotFound("User not found.");
        }

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
        {
            return RedirectToAction("Login");
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAccount()
    {
        if (!User.Identity!.IsAuthenticated)
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
    public IActionResult ExternalLogin(string provider, string? returnUrl = null)
    {
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
        var properties = _userService.ConfigureExternalAuthenticationProperties(provider, redirectUrl!);
        return new ChallengeResult(provider, properties);
    }

    [HttpGet]
    public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
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
            new Claim(ClaimTypes.NameIdentifier, user!.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email)
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
        await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity));

        await HttpContext.SignOutAsync("External"); // Sign out from external cookie

        return RedirectToLocal(returnUrl);
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Home");
    }
}
