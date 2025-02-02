using System;
using AssetTracker.Models;
namespace AssetTracker.Repositories
{
	public interface IUserRepository
	{

		Task<User> GetUserAsync(int UserId);
		Task<IEnumerable<User>> GetUsersAsync();
		Task AddUserAsync(User user);
		Task UpdateUserAsync(User user);
		Task RemoveUserAsync(int userId);

	}
}

