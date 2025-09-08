using HorsesForCourses.Core;

namespace HorsesForCourses.Test;

public class UserTests
{
    private readonly IPasswordHasher _hasher = new Pbkdf2PasswordHasher();

    [Fact]
    public void User_Constructor()
    {
        var user = new User("John Doe", "john@example.com", "hashedpassword");

        Assert.Equal("John Doe", user.Name);
        Assert.Equal("john@example.com", user.Email);
        Assert.Equal("hashedpassword", user.PasswordHash);
    }

    [Fact]
    public void User_From_ValidData()
    {
        var user = User.From("John Doe", "john@example.com", "password123", "password123", _hasher);

        Assert.Equal("John Doe", user.Name);
        Assert.Equal("john@example.com", user.Email);
        Assert.NotNull(user.PasswordHash);
        Assert.NotEqual("password123", user.PasswordHash); // Should be hashed
    }

    [Fact]
    public void User_From_EmptyName_ThrowsException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            User.From("", "john@example.com", "password123", "password123", _hasher));
        Assert.Equal("Name cannot be empty. (Parameter 'name')", exception.Message);
    }

    [Fact]
    public void User_From_EmptyEmail_ThrowsException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            User.From("John Doe", "", "password123", "password123", _hasher));
        Assert.Equal("Email cannot be empty. (Parameter 'email')", exception.Message);
    }

    [Fact]
    public void User_From_InvalidEmail_ThrowsException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            User.From("John Doe", "invalid-email", "password123", "password123", _hasher));
        Assert.Equal("Invalid email format. (Parameter 'email')", exception.Message);
    }

    [Fact]
    public void User_From_ShortPassword_ThrowsException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            User.From("John Doe", "john@example.com", "123", "123", _hasher));
        Assert.Equal("Password must be at least 6 characters long. (Parameter 'pass')", exception.Message);
    }

    [Fact]
    public void User_From_PasswordsDontMatch_ThrowsException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            User.From("John Doe", "john@example.com", "password123", "different", _hasher));
        Assert.Equal("Passwords do not match. (Parameter 'confirmPass')", exception.Message);
    }

    [Fact]
    public void User_VerifyPassword_ValidPassword()
    {
        var user = User.From("John Doe", "john@example.com", "password123", "password123", _hasher);

        Assert.True(user.VerifyPassword("password123", _hasher));
    }

    [Fact]
    public void User_VerifyPassword_InvalidPassword()
    {
        var user = User.From("John Doe", "john@example.com", "password123", "password123", _hasher);

        Assert.False(user.VerifyPassword("wrongpassword", _hasher));
    }
}