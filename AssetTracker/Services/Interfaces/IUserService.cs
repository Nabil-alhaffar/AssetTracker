using System;
using AssetTracker.Models;

namespace AssetTracker.Services.Interfaces
{
	public interface IUserService
	{
        Task AddUserAsync(User user);
        Task<User> GetUserAsync(Guid userId);
        Task<IEnumerable<User>> GetUsersAsync();
        Task RemoveUsersAsync(Guid userId);
        public Task<User> AuthenticateUserAsync(string username, string password);
        public Task RegisterUserAsync(User user, string password);
        public Task ResetPasswordAsync(Guid userId, string newPassword);
        public Task ResetEmailAsync(Guid userId, string newEmail);
        public Task ResetUsernameAsync(Guid userId, string newUsername);
        public Task ClearRefreshTokenAsync(Guid userId);
        public Task UpdateUserRefreshTokenAsync(Guid userId, string refreshToken, DateTime refreshTokenExpiryTime);



    }
}

