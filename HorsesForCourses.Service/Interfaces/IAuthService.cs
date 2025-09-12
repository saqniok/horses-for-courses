using System.Security.Claims;
using HorsesForCourses.Core;
using HorsesForCourses.Service.DTOs;
using Microsoft.AspNetCore.Authentication;

namespace HorsesForCourses.Service.Interfaces
{
    public interface IAuthService
    {
        Task<(ClaimsPrincipal? ClaimsPrincipal, Dictionary<string, string> Errors)> LoginAsync(LoginDto model);
        Task<(ClaimsPrincipal? ClaimsPrincipal, Dictionary<string, string> Errors)> RegisterAsync(RegisterAccountDto model);
        Task SignOutAsync();
        Task<(ClaimsPrincipal? ClaimsPrincipal, Dictionary<string, string> Errors)> ExternalLoginCallbackAsync(string? returnUrl, string? remoteError);
        AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl);
        Task DeleteAccountAsync(int userId);
        Task<User?> GetUserByEmailAsync(string email);
    }
}
