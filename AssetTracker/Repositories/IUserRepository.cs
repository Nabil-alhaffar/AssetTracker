using System;
using AssetTracker.Models;
namespace AssetTracker.Repositories
{
	public interface IUserRepository
	{

		Task<User> GetUserAsync(Guid UserId);
		Task<IEnumerable<User>> GetUsersAsync();
		Task AddUserAsync(User user);
		Task UpdateUserAsync(User user);
		Task RemoveUserAsync(Guid userId);

	}
}

