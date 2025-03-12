using System;
using System.Security.Cryptography;
using System.Text;
using AssetTracker.Models;
using AssetTracker.Repositories;
using AssetTracker.Services.Interfaces;
using AssetTracker.Repositories.MongoDBRepositories;
using AssetTracker.Repositories.Interfaces;
namespace AssetTracker.Services
{
	public class UserService:IUserService
	{
        private readonly IUserRepository _userRepository;
        private readonly IPortfolioRepository _portfolios;
        private readonly IPasswordService _passwordService;


        public UserService(IPasswordService passwordService , IUserRepository userRepository,  IPortfolioRepository portfolioRepository)
		{
            _userRepository = userRepository;
            _portfolios = portfolioRepository;
            _passwordService = passwordService;

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
                Positions = new Dictionary<string, Position>(),  // Initialize with empty positions
                AvailableFunds =0
            };
            //user.Portfolio = portfolio;
         
            await _portfolios.AddPortfolioAsync(portfolio);  // Add to the portfolio repository too
            await _userRepository.AddUserAsync(user);


            // Add user logic
        }

        public async Task<User> GetUserAsync(Guid userId)
        {
            return await _userRepository.GetUserByIDAsync(userId);
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _userRepository.GetUsersAsync();
        }

        public async Task RemoveUsersAsync(Guid userId)
        {
           await _userRepository.RemoveUserAsync(userId);
        }
        public async Task RegisterUserAsync(User user, string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be empty.");

            var salt = _passwordService.GenerateSalt();
            var hashedPassword = _passwordService.HashPassword(password, salt);

            user.PasswordHash = hashedPassword;
            user.PasswordSalt = salt;

            await AddUserAsync(user);
        }

        // Method to authenticate a user
        public async Task<User> AuthenticateUserAsync(string username, string password)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid username or password.");

            if (!_passwordService.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
                throw new UnauthorizedAccessException("Invalid username or password.");

            return user;
        }


    }
}

