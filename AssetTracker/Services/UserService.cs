using System;
using AssetTracker.Models;
using AssetTracker.Repositories;

namespace AssetTracker.Services
{
	public class UserService:IUserService
	{
        private readonly IUserRepository _userRepository;
		public UserService(IUserRepository userRepository)
		{
            _userRepository = userRepository;
		}

        public async Task AddUserAsync(User user)
        {
            await _userRepository.AddUserAsync(user);
        }

        public async Task<User> GetUserAsync(int userId)
        {
            return await _userRepository.GetUserAsync(userId);
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _userRepository.GetUsersAsync();
        }

        public async Task RemoveUsersAsync(int userId)
        {
            throw new NotImplementedException();
        }
    }
}

