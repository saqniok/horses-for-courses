namespace HorsesForCourses.Core;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    // For EF
    protected User() { }

    public User(string name, string email, string passwordHash)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
    }

    public static User From(string name, string email, string pass, string confirmPass, IPasswordHasher hasher)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));

        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email format.", nameof(email));

        if (string.IsNullOrWhiteSpace(pass))
            throw new ArgumentException("Password cannot be empty.", nameof(pass));

        if (pass.Length < 6)
            throw new ArgumentException("Password must be at least 6 characters long.", nameof(pass));

        if (pass != confirmPass)
            throw new ArgumentException("Passwords do not match.", nameof(confirmPass));

        var passwordHash = hasher.Hash(pass);

        return new User(name, email, passwordHash);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public bool VerifyPassword(string password, IPasswordHasher hasher)
    {
        return hasher.Verify(password, PasswordHash);
    }
}