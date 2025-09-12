using System.Security.Claims;
using HorsesForCourses.Core;
using System.Security.Claims;
using HorsesForCourses.Core;
using HorsesForCourses.Service.DTOs;
using HorsesForCourses.Service.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace HorsesForCourses.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IUserService userService, IPasswordHasher passwordHasher, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(ClaimsPrincipal? ClaimsPrincipal, Dictionary<string, string> Errors)> LoginAsync(LoginDto model)
        {
            var errors = new Dictionary<string, string>();

            var user = await _userService.GetByEmailAsync(model.Email);

            if (user == null || !_passwordHasher.Verify(model.Password, user.PasswordHash))
            {
                errors.Add(string.Empty, "Invalid login attempt.");
                return (null, errors);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await _httpContextAccessor.HttpContext!.SignInAsync("Cookies", claimsPrincipal);

            return (claimsPrincipal, errors);
        }

        public async Task<(ClaimsPrincipal? ClaimsPrincipal, Dictionary<string, string> Errors)> RegisterAsync(RegisterAccountDto model)
        {
            var errors = new Dictionary<string, string>();

            var existingUser = await _userService.GetByEmailAsync(model.Email);
            if (existingUser != null)
            {
                errors.Add("Email", "User with this email already exists.");
                return (null, errors);
            }

            var hashedPassword = _passwordHasher.Hash(model.Password);
            UserRole selectedRole = UserRole.User;

            if (model.IsAdmin)
            {
                selectedRole = UserRole.Admin;
            }
            else if (model.IsCoach)
            {
                selectedRole = UserRole.Coach;
            }

            var newUser = new User(model.Name, model.Email, hashedPassword, selectedRole);
            await _userService.CreateAsync(newUser);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, newUser.Id.ToString()),
                new Claim(ClaimTypes.Name, newUser.Email),
                new Claim(ClaimTypes.Role, newUser.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await _httpContextAccessor.HttpContext!.SignInAsync("Cookies", claimsPrincipal);

            return (claimsPrincipal, errors);
        }

        public async Task SignOutAsync()
        {
            await _httpContextAccessor.HttpContext!.SignOutAsync("Cookies");
        }

        public async Task<(ClaimsPrincipal? ClaimsPrincipal, Dictionary<string, string> Errors)> ExternalLoginCallbackAsync(string? returnUrl, string? remoteError)
        {
            var errors = new Dictionary<string, string>();

            if (remoteError != null)
            {
                errors.Add(string.Empty, $"Error from external provider: {remoteError}");
                return (null, errors);
            }

            var info = await _httpContextAccessor.HttpContext!.AuthenticateAsync("External");
            if (info?.Principal == null)
            {
                errors.Add(string.Empty, "External authentication failed.");
                return (null, errors);
            }

            var emailClaim = info.Principal.FindFirst(ClaimTypes.Email);
            if (emailClaim == null)
            {
                errors.Add(string.Empty, "External authentication provider did not provide an email address.");
                return (null, errors);
            }

            var email = emailClaim.Value;
            var user = await _userService.GetByEmailAsync(email);

            if (user == null)
            {
                var nameClaim = info.Principal.FindFirst(ClaimTypes.Name);
                var name = nameClaim?.Value ?? email;

                var newUser = new User(name, email, _passwordHasher.Hash(Guid.NewGuid().ToString()));
                await _userService.CreateAsync(newUser);
                user = newUser;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user!.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await _httpContextAccessor.HttpContext!.SignInAsync("Cookies", claimsPrincipal);
            await _httpContextAccessor.HttpContext.SignOutAsync("External");

            return (claimsPrincipal, errors);
        }

        public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl)
        {
            return new AuthenticationProperties { RedirectUri = redirectUrl };
        }

        public async Task DeleteAccountAsync(int userId)
        {
            await _userService.DeleteAsync(userId);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userService.GetByEmailAsync(email);
        }
    }
}
