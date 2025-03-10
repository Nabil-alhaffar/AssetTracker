using System;
using System.Threading.Tasks;
using AssetTracker.Models;
using AssetTracker.Services;
using Microsoft.AspNetCore.Mvc;
using AssetTracker.Services.Interfaces;
namespace AssetTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public AuthController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
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

                //// Create session ID
                //var sessionId = Guid.NewGuid().ToString();  // Generate a session ID

                //// Optionally store the session ID in your database or cache (e.g., Redis)
                //await _userService.SaveSessionIdAsync(user.UserId, sessionId); // Hypothetical method to store session ID

                // Return the response with the session ID and JWT token
                return Ok(new
                {
                    message = "Login successful",
                    userId = user.UserId,
                    //sessionId = sessionId,  // Add session ID to the response
                    token = token           // Include JWT token for further requests
                });
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
