using System;
using AssetTracker.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace AssetTracker.Services
{
    public class PasswordService : IPasswordService
    {
        // Method to hash password using PBKDF2
        public string HashPassword(string password, string salt)
        {
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(salt)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hash);
            }
        }

        // Method to verify the password
        public bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            var hashedPassword = HashPassword(password, storedSalt);
            return storedHash == hashedPassword;
        }

        // Generate a secure salt
        public string GenerateSalt()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] salt = new byte[16]; // 16 bytes of salt
                rng.GetBytes(salt); // Fill the salt array with random bytes
                return Convert.ToBase64String(salt); // Convert to a base64 string for storage
            }
        }
    }
}


