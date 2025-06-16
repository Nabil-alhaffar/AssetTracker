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
using AssetTracker.Models.DTOs;

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
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
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
                await Login( new LoginRequest { UserName=  model.UserName, Password =model.Password });

                return Ok(new { message = "User registered and Logged in successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest model)
        {
            try
            {
                // Try to get token from Authorization header
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new { message = "Missing token" });
                }

                var token = authHeader.Substring("Bearer ".Length);

                // Use helper to get claims from expired token
                var principal = JwtHelpers.GetPrincipalFromExpiredToken(token, _configuration); // You implement this
                var userId = principal.GetUserId(); // Your extension method

                await _symbolSubscriptionManager.UnsubscribeUserFromAllAsync(userId);
                await _userSessionManager.EndSessionAsync(userId, model.SessionId);
                Response.Cookies.Delete("refreshToken");
                await _userService.ClearRefreshTokenAsync(userId);
                return Ok(new { message = "Logout successful (from expired token)" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        //[HttpPost("logout")]
        //[AllowAnonymous]
        //public async Task<IActionResult>Logout([FromBody] LogoutModel model)
        //{
        //    try
        //    {
        //        var userId = User.GetUserId(); // Retrieve user ID from JWT claims

        //        // Unsubscribe user from all symbols
        //        await _symbolSubscriptionManager.UnsubscribeUserFromAllAsync(userId);

        //        // Optional: Clear session state or in-memory data (if implemented)
        //        await _userSessionManager.EndSessionAsync(userId, model.SessionId);

        //        return Ok(new { message = "Logout successful" });
        //    }
        //    catch (UnauthorizedAccessException)
        //    {
        //        return Unauthorized(new { message = "User is not authenticated" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }

        //}

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
         
                // Authenticate user
                var user = await _userService.AuthenticateUserAsync(request.UserName, request.Password);

                // Generate JWT token
                var token = _authService.GenerateJwtToken(user);
                var refreshToken = _authService.GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _userService.UpdateUserRefreshTokenAsync(user.UserId, refreshToken, (DateTime)user.RefreshTokenExpiryTime);
                var sessionId = Guid.NewGuid().ToString();

                var watchlistsSymbols = await _watchlistService.GetAllWatchedTickersByUserIdAsync(user.UserId);
                var positionSymbols = (await _portfolioService.GetPortfolioPositionsAsync(user.UserId)).Keys.ToList();
                var combinedList = watchlistsSymbols.Concat(positionSymbols).Distinct();
                foreach(string symbol in combinedList)
                {
                    await _symbolSubscriptionManager.SubscribeUserToSymbolAsync(user.UserId, symbol);
                }
                if (request.TimeZoneId != null && request.TimeZoneId!=user.TimeZoneId)
                {
                    user.TimeZoneId = request.TimeZoneId;
                    await _userService.UpdateUserTimeZoneAsync(user.UserId, request.TimeZoneId);
                }
                await _userSessionManager.StartSessionAsync(user.UserId, sessionId, Request.HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers["User-Agent"]);

                Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Only over HTTPS
                    SameSite = SameSiteMode.None, // Or Lax if you're supporting cross-site auth
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    IsEssential = true,
                    Path = "/",
                    Domain = "ec2-18-188-45-142.us-east-2.compute.amazonaws.com"


                }) ;

                return Ok(new LoginResponse 
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    SessionId = sessionId,  
                    Token = token,          
                    TimeZoneId = user.TimeZoneId,
                    Message = "Login Successful!"
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
        public async Task<IActionResult> Refresh()
        {
            try
            {
                var authHeader = Request.Headers["Authorization"].ToString();

                if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new { message = "Missing or invalid authorization header" });
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();

                var principal = JwtHelpers.GetPrincipalFromExpiredToken(token, _configuration);
                var userIdStr = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
                    return Unauthorized();

                var user = await _userService.GetUserAsync(userId);
                var refreshToken = Request.Cookies["refreshToken"];

                if (string.IsNullOrEmpty(refreshToken) || user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                {
                    return Unauthorized();
                }

                var newJwt = _authService.GenerateJwtToken(user);
                var newRefreshToken = _authService.GenerateRefreshToken();

                await _userService.UpdateUserRefreshTokenAsync(user.UserId, newRefreshToken, DateTime.UtcNow.AddDays(7));

                Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    IsEssential= true,
                    Path="/",
                    Domain = "ec2-18-188-45-142.us-east-2.compute.amazonaws.com"
                });

                return Ok(new
                {
                    token = newJwt
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
        //[HttpPost("refresh-token")]
        //[AllowAnonymous]
        //public async Task<IActionResult> Refresh([FromBody] TokenRequest request)
        //{
        //    try
        //    {
        //        var principal = JwtHelpers.GetPrincipalFromExpiredToken(request.Token, _configuration);
        //        var userIdStr = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
        //            return Unauthorized();

        //        var user = await _userService.GetUserAsync(userId);
        //        var refreshToken = Request.Cookies["refreshToken"];

        //        if (string.IsNullOrEmpty(refreshToken) || user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
        //        {
        //            return Unauthorized();
        //        }

        //        var newJwt = _authService.GenerateJwtToken(user);
        //        var newRefreshToken = _authService.GenerateRefreshToken();

        //        await _userService.UpdateUserRefreshTokenAsync(user.UserId, newRefreshToken, DateTime.UtcNow.AddDays(7));

        //        Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
        //        {
        //            HttpOnly = true,
        //            Secure = true,
        //            SameSite = SameSiteMode.Strict,
        //            Expires = DateTimeOffset.UtcNow.AddDays(7),
        //            IsEssential = true

        //        }) ;


        //        return Ok(new
        //        {
        //            token = newJwt,
        //            //refreshToken = newRefreshToken
        //        });
        //    }
        //    catch (SecurityTokenException ex)
        //    {
        //        return Unauthorized(new { message = "Invalid or expired token." });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = ex.Message });
        //    }
        //}
    }



}
