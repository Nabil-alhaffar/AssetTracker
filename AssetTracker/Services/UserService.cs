using System;
using AssetTracker.Models;
using AssetTracker.Repositories;

namespace AssetTracker.Services
{
	public class UserService:IUserService
	{
        private readonly IUserRepository _userRepository;
        private readonly IPortfolioRepository _portfolios;
        
		public UserService(IUserRepository userRepository, IPortfolioRepository portfolioRepository)
		{
            _userRepository = userRepository;
            _portfolios = portfolioRepository;
		}

        public async Task AddUserAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            var portfolio = new Portfolio
            {
                UserId = user.UserId,  // Link the portfolio to the user
                Positions = new List<Position>()  // Initialize with empty positions
               
            };
            user.Portfolio = portfolio;

            await _portfolios.AddPortfolioAsync(portfolio);  // Add to the portfolio repository too
            await _userRepository.AddUserAsync(user);


            // Add user logic
        }

        public async Task<User> GetUserAsync(Guid userId)
        {
            return await _userRepository.GetUserAsync(userId);
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _userRepository.GetUsersAsync();
        }

        public async Task RemoveUsersAsync(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}

