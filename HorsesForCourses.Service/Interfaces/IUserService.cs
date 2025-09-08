using HorsesForCourses.Core;

namespace HorsesForCourses.Service.Interfaces;

public interface IUserService
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task CreateAsync(User user);
}