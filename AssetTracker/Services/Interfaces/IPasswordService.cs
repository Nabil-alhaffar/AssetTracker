using System;

namespace AssetTracker.Services.Interfaces
{
    /// <summary>
    /// Provides password hashing, verification, and salt generation services.
    /// </summary>
    public interface IPasswordService
    {
        /// <summary>
        /// Hashes a password using the provided salt.
        /// </summary>
        /// <param name="password">The plain text password to hash.</param>
        /// <param name="salt">The salt to use during hashing.</param>
        /// <returns>The hashed password as a string.</returns>
        string HashPassword(string password, string salt);

        /// <summary>
        /// Verifies whether a given plain password matches the stored hash and salt.
        /// </summary>
        /// <param name="password">The plain text password to verify.</param>
        /// <param name="storedHash">The stored hashed password.</param>
        /// <param name="storedSalt">The stored salt used for hashing.</param>
        /// <returns>True if the password is verified; otherwise, false.</returns>
        bool VerifyPassword(string password, string storedHash, string storedSalt);

        /// <summary>
        /// Generates a new cryptographic salt.
        /// </summary>
        /// <returns>A new salt string.</returns>
        string GenerateSalt();
    }
}