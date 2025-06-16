using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AssetTracker.Models;
using AssetTracker.Repositories;
using AssetTracker.Repositories.Interfaces;
using AssetTracker.Repositories.MongoDBRepositories;
using AssetTracker.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AssetTracker.Services
{

    /// <summary>
    /// Handles user authentication and JWT/refresh token generation.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IPasswordService passwordService, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _configuration = configuration;
        }

        /// <summary>
        /// Authenticates a user based on username and password.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>The authenticated <see cref="User"/> object if successful; otherwise, null.</returns>
        public async Task<User> AuthenticateUserAsync(string username, string password)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null || !_passwordService.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        /// <summary>
        /// Generates a JSON Web Token (JWT) for the specified user.
        /// </summary>
        /// <param name="user">The user for whom the JWT will be generated.</param>
        /// <returns>A signed JWT token string.</returns>
        public string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        /// <summary>
        /// Generates a cryptographically secure refresh token.
        /// </summary>
        /// <returns>A Base64-encoded refresh token string.</returns>
        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
