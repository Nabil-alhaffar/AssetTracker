using System;
using AssetTracker.Models;

namespace AssetTracker.Services
{
	public interface IUserService
	{
        Task AddUserAsync(User user);
        Task<User> GetUserAsync(Guid userId);
        Task<IEnumerable<User>> GetUsersAsync();
        Task RemoveUsersAsync(Guid userId);
    }
}

