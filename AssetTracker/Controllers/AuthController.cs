using System;
using System.Threading.Tasks;
using AssetTracker.Models;
using AssetTracker.Services;
using Microsoft.AspNetCore.Mvc;
using AssetTracker.Services.Interfaces;
using AssetTracker.Helpers;
namespace AssetTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IWatchlistService _watchlistService;
        private readonly IPortfolioService _portfolioService;
        private readonly IUserSessionManager _userSessionManager;
        private readonly SymbolSubscriptionManager _symbolSubscriptionManager;
        public AuthController(IUserService userService, IAuthService authService, SymbolSubscriptionManager symbolSubscriptionManager,
                              IWatchlistService watchlistService, IPortfolioService portfolioService, IUserSessionManager userSessionManager)
        {
            _userService = userService;
            _authService = authService;
            _watchlistService = watchlistService;
            _portfolioService = portfolioService;
            _symbolSubscriptionManager = symbolSubscriptionManager;
            _userSessionManager = userSessionManager;
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
                    Email = model.Email
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

        // Endpoint to authenticate a user

        [HttpPost("logout")]
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

                var sessionId = Guid.NewGuid().ToString();

                var watchlistsSymbols = await _watchlistService.GetAllWatchedTickersByUserIdAsync(user.UserId);
                var positionSymbols = (await _portfolioService.GetPortfolioPositionsAsync(user.UserId)).Keys.ToList();
                var combinedList = watchlistsSymbols.Concat(positionSymbols).Distinct();
                foreach(string symbol in combinedList)
                {
                    await _symbolSubscriptionManager.SubscribeUserToSymbolAsync(user.UserId, symbol);
                }

                await _userSessionManager.StartSessionAsync(user.UserId, sessionId, Request.HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers["User-Agent"]);


                return Ok(new
                {
                    message = "Login successful",
                    userId = user.UserId,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    email = user.Email,
                    sessionId = sessionId,  // Add session ID to the response
                    token = token           // Include JWT token for further requests
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
    }

    // Model for user registration


    // Model for user login

}
