using System;
using System.Security.Cryptography;
using System.Text;
using AssetTracker.Models;
using AssetTracker.Repositories;
using AssetTracker.Services.Interfaces;
using AssetTracker.Repositories.MongoDBRepositories;
using AssetTracker.Repositories.Interfaces;
using System.ComponentModel.DataAnnotations;

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
            if (string.IsNullOrWhiteSpace(password?.Trim()))
                throw new ArgumentException("Password cannot be empty or whitespace.");

            var salt = _passwordService.GenerateSalt();
            var hashedPassword = _passwordService.HashPassword(password, salt);


            user.PasswordHash = hashedPassword;
            user.PasswordSalt = salt;

            var emailValidator = new EmailAddressAttribute();
            if (!emailValidator.IsValid(user.Email))
                throw new ArgumentException("Invalid email format.");
            var existingUser = await _userRepository.GetUserByEmailAsync(user.Email);
            if (existingUser != null && existingUser.UserId != user.UserId)
                throw new InvalidOperationException("Email is already in use.");
            await AddUserAsync(user);
        }

        public async Task ResetPassword(Guid userId, string newPassword) 
        {
            var user = await GetUserAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found.");
            if (string.IsNullOrEmpty(newPassword))
                throw new ArgumentException("Password cannot be empty.");
            if (string.IsNullOrWhiteSpace(newPassword?.Trim()))
                throw new ArgumentException("Password cannot be empty or whitespace.");

            var salt = _passwordService.GenerateSalt();
            var hashedPassword = _passwordService.HashPassword(newPassword, salt);
            user.PasswordSalt = salt;
            user.PasswordHash = hashedPassword;
            await _userRepository.UpdateUserAsync(user);
        }
        public async Task ResetUsername(Guid userId, string newUsername)
        {
            var user = await GetUserAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found.");
            if (string.IsNullOrWhiteSpace(newUsername?.Trim()))
                throw new ArgumentException("Username cannot be empty or whitespace.");
            if (string.IsNullOrEmpty(newUsername))
                throw new ArgumentException("Password cannot be empty.");
            user.UserName = newUsername;
            await _userRepository.UpdateUserAsync(user);
        }
        public async Task ResetEmail(Guid userId, string newEmail)
        {
            var user = await GetUserAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found.");
            
            if (string.IsNullOrEmpty(newEmail))
                throw new ArgumentException("Password cannot be empty.");

            var emailValidator = new EmailAddressAttribute();
            if (!emailValidator.IsValid(newEmail))
                throw new ArgumentException("Invalid email format.");
            var existingUser = await _userRepository.GetUserByEmailAsync(newEmail);
            if (existingUser != null && existingUser.UserId != userId)
                throw new InvalidOperationException("Email is already in use.");

            user.Email = newEmail;
            await _userRepository.UpdateUserAsync(user);
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

