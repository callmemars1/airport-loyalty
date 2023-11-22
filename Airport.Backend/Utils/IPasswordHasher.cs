using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Airport.Backend.Utils;

public record HashedPasswordData(string PasswordHash, string Salt);

public interface IPasswordHasher
{
    public HashedPasswordData HashPassword(string password);

    public bool CheckPassword(string password, HashedPasswordData hashedData);
}

public class HMACSHA256PasswordHasher : IPasswordHasher
{
    public HashedPasswordData HashPassword(string password)
    {
        // Generate a 128-bit salt using a sequence of
        // cryptographically strong random bytes.
        var salt = RandomNumberGenerator.GetBytes(128 / 8); // divide by 8 to convert bits to bytes

        // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
        var hashed = GetBase64HashedPassword(password, salt);

        var saltBase64 = Convert.ToBase64String(salt);

        return new HashedPasswordData(hashed, saltBase64);
    }

    public bool CheckPassword(string password, HashedPasswordData hashedData)
    {
        var salt = Convert.FromBase64String(hashedData.Salt);

        return GetBase64HashedPassword(password, salt) == hashedData.PasswordHash;
    }

    private static string GetBase64HashedPassword(string password, byte[] salt) =>
        Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));
}