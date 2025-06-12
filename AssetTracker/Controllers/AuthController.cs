using System;
using System.Threading.Tasks;
using AssetTracker.Models;
using AssetTracker.Services;
using Microsoft.AspNetCore.Mvc;
using AssetTracker.Services.Interfaces;
using AssetTracker.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AssetTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IWatchlistService _watchlistService;
        private readonly IPortfolioService _portfolioService;
        private readonly IUserSessionManager _userSessionManager;
        private readonly SymbolSubscriptionManager _symbolSubscriptionManager;
        public AuthController(IUserService userService, IAuthService authService, SymbolSubscriptionManager symbolSubscriptionManager,
                              IWatchlistService watchlistService, IPortfolioService portfolioService, IUserSessionManager userSessionManager
                              , IConfiguration configuration)
        {
            _userService = userService;
            _authService = authService;
            _watchlistService = watchlistService;
            _portfolioService = portfolioService;
            _symbolSubscriptionManager = symbolSubscriptionManager;
            _userSessionManager = userSessionManager;
            _configuration = configuration;
        }

        // Endpoint to register a new user
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Register user and create password hash
                var user = new User
                {
                    UserId = Guid.NewGuid(),
                    UserName = model.UserName,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    TimeZoneId = model.TimeZoneId
                };

                await _userService.RegisterUserAsync(user, model.Password);
                await Login( new LoginModel { UserName=  model.UserName, Password =model.Password });

                return Ok(new { message = "User registered and Logged in successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("logout")]
        [AllowAnonymous]
        public async Task<IActionResult>Logout([FromBody] LogoutModel model)
        {
            try
            {
                var userId = User.GetUserId(); // Retrieve user ID from JWT claims

                // Unsubscribe user from all symbols
                await _symbolSubscriptionManager.UnsubscribeUserFromAllAsync(userId);

                // Optional: Clear session state or in-memory data (if implemented)
                await _userSessionManager.EndSessionAsync(userId, model.SessionId);

                return Ok(new { message = "Logout successful" });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "User is not authenticated" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
         
                // Authenticate user
                var user = await _userService.AuthenticateUserAsync(model.UserName, model.Password);

                // Generate JWT token
                var token = _authService.GenerateJwtToken(user);
                var refreshToken = _authService.GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
                await _userService.UpdateUserRefreshTokenAsync(user.UserId, refreshToken, (DateTime)user.RefreshTokenExpiryTime);
                var sessionId = Guid.NewGuid().ToString();

                var watchlistsSymbols = await _watchlistService.GetAllWatchedTickersByUserIdAsync(user.UserId);
                var positionSymbols = (await _portfolioService.GetPortfolioPositionsAsync(user.UserId)).Keys.ToList();
                var combinedList = watchlistsSymbols.Concat(positionSymbols).Distinct();
                foreach(string symbol in combinedList)
                {
                    await _symbolSubscriptionManager.SubscribeUserToSymbolAsync(user.UserId, symbol);
                }
                if (model.TimeZoneId != null && model.TimeZoneId!=user.TimeZoneId)
                {
                    user.TimeZoneId = model.TimeZoneId;
                    await _userService.UpdateUserTimeZoneAsync(user.UserId, model.TimeZoneId);
                }
                await _userSessionManager.StartSessionAsync(user.UserId, sessionId, Request.HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers["User-Agent"]);


                return Ok(new
                {
                    message = "Login successful",
                    userId = user.UserId,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    email = user.Email,
                    sessionId = sessionId,  
                    token = token,          
                    refreshToken = refreshToken,
                    timeZoneId = user.TimeZoneId
                }) ;
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] TokenRequest request)
        {
            try
            {
                var principal = JwtHelpers.GetPrincipalFromExpiredToken(request.Token, _configuration);
                var userIdStr = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
                    return Unauthorized();

                var user = await _userService.GetUserAsync(userId);
                if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                {
                    return Unauthorized();
                }

                var newJwt = _authService.GenerateJwtToken(user);
                var newRefreshToken = _authService.GenerateRefreshToken();

                await _userService.UpdateUserRefreshTokenAsync(user.UserId, newRefreshToken, DateTime.UtcNow.AddDays(7));

                return Ok(new
                {
                    token = newJwt,
                    refreshToken = newRefreshToken
                });
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized(new { message = "Invalid or expired token." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }



}
