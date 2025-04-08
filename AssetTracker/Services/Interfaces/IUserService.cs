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
        public Task ResetPassword(Guid userId, string newPassword);
        public Task ResetEmail(Guid userId, string newEmail);
        public Task ResetUsername(Guid userId, string newUsername);



    }
}

