using System;
using AssetTracker.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace AssetTracker.Services
{
    /// <summary>
    /// Service for handling password operations including hashing and verifying. 
    /// </summary>
    public class PasswordService : IPasswordService
    {


        /// <summary>
        /// Hashes a password using HMACSHA512 with a provided salt.
        /// </summary>
        /// <param name="password">The plain-text password to hash.</param>
        /// <param name="salt">The salt to use in hashing.</param>
        /// <returns>The Base64-encoded hashed password.</returns>
        public string HashPassword(string password, string salt)
        {
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(salt)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hash);
            }
        }


        /// <summary>
        /// Verifies whether a provided password matches the stored hash using the stored salt.
        /// </summary>
        /// <param name="password">The input password to verify.</param>
        /// <param name="storedHash">The previously stored hashed password.</param>
        /// <param name="storedSalt">The salt used during the original hashing.</param>
        /// <returns>True if the password is correct; otherwise, false.</returns>
        public bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            var hashedPassword = HashPassword(password, storedSalt);
            return storedHash == hashedPassword;
        }

        /// <summary>
        /// Generates a cryptographically secure random salt.
        /// </summary>
        /// <returns>A Base64-encoded string representing the salt.</returns>
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


