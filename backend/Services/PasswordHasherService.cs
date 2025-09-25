using System.Security.Cryptography;
using System.Text;

namespace backend.Services
{
    public class PasswordHasherService
    {
        public static readonly int PasswordSaltLength = 64;
        public static readonly int PasswordHashLength = 256;
        public static readonly int Iterations = 400000;

        public string GenerateSalt()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(PasswordSaltLength - 32));
        }

        public string CalculateHash(string password, string salt)
        {
            return Convert.ToBase64String(
                Rfc2898DeriveBytes.Pbkdf2(
                    Encoding.Unicode.GetBytes(password),
                    Convert.FromBase64String(salt),
                    Iterations,
                    HashAlgorithmName.SHA512,
                    PasswordHashLength - 128
                )
            );
        }

        public bool VerifyPassword(string storedHash, string password, string salt)
        {
            return storedHash == CalculateHash(password, salt);
        }
    }
}
