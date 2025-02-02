using System;
using AssetTracker.Models;

namespace AssetTracker.Services
{
	public interface IUserService
	{
        Task AddUserAsync(User user);
        Task<User> GetUserAsync(int userId);
        Task<IEnumerable<User>> GetUsersAsync();
        Task RemoveUsersAsync(int userId);
    }
}

