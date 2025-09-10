using HorsesForCourses.Core;
using Microsoft.AspNetCore.Authentication;

namespace HorsesForCourses.Service.Interfaces;

public interface IUserService
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task CreateAsync(User user);
    Task DeleteAsync(int userId);
    AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl);
}
