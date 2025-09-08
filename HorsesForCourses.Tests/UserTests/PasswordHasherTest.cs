using HorsesForCourses.Core;

namespace HorsesForCourses.Test;

public class PasswordHasherTest
{
    private readonly IPasswordHasher _hasher = new Pbkdf2PasswordHasher();

    [Fact]
    public void Hash_ShouldReturnNonEmptyString()
    {
        var hash = _hasher.Hash("password123");

        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
        Assert.NotEqual("password123", hash);
    }

    [Fact]
    public void Hash_ShouldReturnDifferentHashesForSamePassword()
    {
        var hash1 = _hasher.Hash("password123");
        var hash2 = _hasher.Hash("password123");

        Assert.NotEqual(hash1, hash2); // Due to random salt
    }

    [Fact]
    public void Verify_ShouldReturnTrueForCorrectPassword()
    {
        var hash = _hasher.Hash("password123");

        Assert.True(_hasher.Verify("password123", hash));
    }

    [Fact]
    public void Verify_ShouldReturnFalseForIncorrectPassword()
    {
        var hash = _hasher.Hash("password123");

        Assert.False(_hasher.Verify("wrongpassword", hash));
    }

    [Fact]
    public void Verify_ShouldReturnFalseForInvalidHash()
    {
        Assert.False(_hasher.Verify("password123", "invalidhash"));
        Assert.False(_hasher.Verify("password123", ""));
        Assert.False(_hasher.Verify("password123", "invalid.format"));
    }
}