using System;
using AssetTracker.Models;
namespace AssetTracker.Repositories.Interfaces
{
	public interface IUserRepository
	{

		Task<User> GetUserByIDAsync(Guid userId);
		Task<User> GetUserByUsernameAsync(string username);
		Task<IEnumerable<User>> GetUsersAsync();
		Task AddUserAsync(User user);
		Task UpdateUserAsync(User user);
		Task RemoveUserAsync(Guid userId);

	}
}

