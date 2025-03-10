using System;
using AssetTracker.Models;

namespace AssetTracker.Services.Interfaces
{
	public interface IAuthService
	{
        public Task<User> AuthenticateUserAsync(string username, string password);

        public string GenerateJwtToken(User user);


    }
}

