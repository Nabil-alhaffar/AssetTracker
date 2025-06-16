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

    /// <summary>
    /// Provides services related to user management such as registration, authentication,
    /// profile updates, and portfolio initialization.
    /// </summary>
    public class UserService:IUserService
	{
        private readonly IUserRepository _userRepository;
        private readonly IPortfolioRepository _portfolios;
        private readonly IPasswordService _passwordService;


        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="passwordService">The password hashing and verification service.</param>
        /// <param name="userRepository">Repository to manage user data.</param>
        /// <param name="portfolioRepository">Repository to manage portfolio data.</param>
        public UserService(IPasswordService passwordService , IUserRepository userRepository,  IPortfolioRepository portfolioRepository)
		{
            _userRepository = userRepository;
            _portfolios = portfolioRepository;
            _passwordService = passwordService;

        }

        /// <summary>
        /// Adds a user and initializes their portfolio.
        /// </summary>
        /// <param name="user">The user to add.</param>
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

        /// <summary>
        /// Retrieves a user by ID.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The user object.</returns>
        public async Task<User> GetUserAsync(Guid userId)
        {
            return await _userRepository.GetUserByIDAsync(userId);
        }


        /// <summary>
        /// Retrieves all users.
        /// </summary>
        /// <returns>A collection of all users.</returns>
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _userRepository.GetUsersAsync();
        }


        /// <summary>
        /// Removes a user by ID.
        /// </summary>
        /// <param name="userId">The ID of the user to remove.</param>
        public async Task RemoveUsersAsync(Guid userId)
        {
           await _userRepository.RemoveUserAsync(userId);
        }

        /// <summary>
        /// Registers a new user with a password.
        /// </summary>
        /// <param name="user">The user to register.</param>
        /// <param name="password">The password to hash and store.</param>
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


        /// <summary>
        /// Resets the password for a given user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="newPassword">The new password to set.</param>
        public async Task ResetPasswordAsync(Guid userId, string newPassword) 
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

        /// <summary>
        /// Resets the username for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="newUsername">The new username.</param>
        public async Task ResetUsernameAsync(Guid userId, string newUsername)
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

        /// <summary>
        /// Resets the email address for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="newEmail">The new email address.</param>
        public async Task ResetEmailAsync(Guid userId, string newEmail)
        {
            var user = await GetUserAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found.");
            
            if (string.IsNullOrEmpty(newEmail))
                throw new ArgumentException("New email cannot be empty.");

            var emailValidator = new EmailAddressAttribute();
            if (!emailValidator.IsValid(newEmail))
                throw new ArgumentException("Invalid email format.");
            var existingUser = await _userRepository.GetUserByEmailAsync(newEmail);
            if (existingUser != null && existingUser.UserId != userId)
                throw new InvalidOperationException("Email is already in use.");

            user.Email = newEmail;
            await _userRepository.UpdateUserAsync(user);
        }

        /// <summary>
        /// Authenticates a user by username and password.
        /// </summary>
        /// <param name="username">The user's username.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>The authenticated user object.</returns>
        public async Task<User> AuthenticateUserAsync(string username, string password)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid username or password.");

            if (!_passwordService.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
                throw new UnauthorizedAccessException("Invalid username or password.");

            return user;
        }


        /// <summary>
        /// Updates the refresh token for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="refreshToken">The new refresh token.</param>
        /// <param name="refreshTokenExpiryTime">The expiry time of the refresh token.</param>
        public async Task UpdateUserRefreshTokenAsync(Guid userId, string refreshToken,  DateTime refreshTokenExpiryTime)
        {
            try
            {
                var user = await _userRepository.GetUserByIDAsync(userId);
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = refreshTokenExpiryTime;
                await _userRepository.UpdateUserAsync(user);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update user refresh token", ex);
            }

        }

        /// <summary>
        /// Clears the refresh token for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        public async Task ClearRefreshTokenAsync(Guid userId)
        {
            try
            {
                var user = await GetUserAsync(userId);

                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = default;
                await _userRepository.UpdateUserAsync(user);
            }
            catch (Exception ex)
            {
                throw new Exception("Refresh token could not be cleared:", ex);
            }

        }


        /// <summary>
        /// Updates the user's preferred time zone.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="timeZoneId">The time zone identifier (e.g., "America/New_York").</param>
        public async Task UpdateUserTimeZoneAsync(Guid userId, string timeZoneId)
        {
            try
            {
                var user = await GetUserAsync(userId);
                user.TimeZoneId = timeZoneId;
                await _userRepository.UpdateUserAsync(user);
            }

            catch (Exception ex)
            {
                throw new Exception("Error updating timezone: " ,ex);
            }

        }



    }
}

