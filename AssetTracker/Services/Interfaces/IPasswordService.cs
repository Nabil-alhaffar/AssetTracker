using System;
namespace AssetTracker.Services.Interfaces
{
	public interface IPasswordService
	{
        string HashPassword(string password, string salt);
        bool VerifyPassword(string password, string storedHash, string storedSalt);
        string GenerateSalt();
    }
}

