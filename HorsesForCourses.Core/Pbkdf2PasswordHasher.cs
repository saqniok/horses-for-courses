using System.Security.Cryptography;
using System.Text;

namespace HorsesForCourses.Core;

public class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16; // 128 bits
    private const int KeySize = 32; // 256 bits
    private const int Iterations = 10000; // Number of iterations for PBKDF2

    public string Hash(string password)
    {
        using (var algorithm = new Rfc2898DeriveBytes(
            password,
            SaltSize,
            Iterations,
            HashAlgorithmName.SHA256))
        {
            var salt = algorithm.Salt;
            var key = algorithm.GetBytes(KeySize);

            // Combine salt and key, then convert to base64 for storage
            var hashBytes = new byte[SaltSize + KeySize];
            Buffer.BlockCopy(salt, 0, hashBytes, 0, SaltSize);
            Buffer.BlockCopy(key, 0, hashBytes, SaltSize, KeySize);

            return Convert.ToBase64String(hashBytes);
        }
    }

    public bool Verify(string password, string hash)
    {
        try
        {
            var hashBytes = Convert.FromBase64String(hash);

            if (hashBytes.Length != SaltSize + KeySize)
            {
                return false; // Invalid hash length
            }

            var salt = new byte[SaltSize];
            Buffer.BlockCopy(hashBytes, 0, salt, 0, SaltSize);

            var storedKey = new byte[KeySize];
            Buffer.BlockCopy(hashBytes, SaltSize, storedKey, 0, KeySize);

            using (var algorithm = new Rfc2898DeriveBytes(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256))
            {
                var keyToCheck = algorithm.GetBytes(KeySize);
                return keyToCheck.SequenceEqual(storedKey);
            }
        }
        catch (FormatException)
        {
            return false; // Invalid Base64 string
        }
        catch (ArgumentException)
        {
            return false; // Invalid arguments (e.g., null password or hash)
        }
    }
}
