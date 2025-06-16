    using System;
using AssetTracker.Models;
using AssetTracker.Services;
using Microsoft.AspNetCore.Mvc;
using AssetTracker.Services.Interfaces;
using AssetTracker.Helpers;

namespace AssetTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <returns>User object if found, otherwise 404 Not Found</returns>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            var user = await _userService.GetUserAsync(userId);
            if (user == null) return NotFound("User not found");
            return Ok(user);
        }


        /// <summary>
        /// Retrieves a list of all registered users.
        /// </summary>
        /// <returns>List of users and the count, or 404 if none exist</returns>
        [HttpGet("Get-all")]
        public async Task <IActionResult> GetAllUsers()
        {
            var users = await _userService.GetUsersAsync();

            if (!users.Any())
                return NotFound(new { message = "No users found." });

            return Ok(new { count = users.Count(), users });

        }

        /// <summary>
        /// Resets the password for the specified user.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <param name="request">New password to be set</param>
        /// <returns>Confirmation message or error details</returns>
        [HttpPatch("{userId}/reset-password")]
        public async Task <IActionResult> ResetPassword (Guid userId,[FromBody] ResetPasswordRequest request)
        {
            try
            {
                await _userService.ResetPasswordAsync(userId, request.NewPassword);

                return Ok("Password has been successfully reset");

            }
            catch (Exception ex)
            {
                return BadRequest($"Error resetting password: {ex.Message}");
            }
        }


        /// <summary>
        /// Resets the email address for the specified user.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <param name="request">New email to be set</param>
        /// <returns>Confirmation message or error details</returns>
        [HttpPatch("{userId}/reset-email")]
        public async Task<IActionResult> ResetEmail(Guid userId, [FromBody] ResetEmailRequest request)
        {
            try
            {
                await _userService.ResetEmailAsync(userId, request.NewEmail);
                return Ok("Email has been successfully reset");

            }
            catch (Exception ex)
            {
                return BadRequest($"Error resetting email: {ex.Message}");
            }
        }


        /// <summary>
        /// Resets the username for the specified user.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <param name="request">New username to be set</param>
        /// <returns>Confirmation message or error details</returns>
        [HttpPatch("{userId}/reset-username")]
        public async Task<IActionResult> ResetUsername(Guid userId, [FromBody] ResetUsernameRequest request)
        {
            try
            {
                await _userService.ResetUsernameAsync(userId, request.NewUsername);
                return Ok("Username has been successfully reset");

            }
            catch (Exception ex)
            {
                return BadRequest($"Error resetting username: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the time zone setting for the specified user.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <param name="model">Time zone information</param>
        /// <returns>Confirmation message on successful update</returns>
        [HttpPut("{userId}/timezone")]
        public async Task<IActionResult> UpdateTimeZone( Guid userId, [FromBody] TimeZoneUpdateRequest model)
        {
            await _userService.UpdateUserTimeZoneAsync(userId, model.TimeZoneId);
            return Ok(new { message = "Time zone updated successfully" });
        }
        //[HttpPost("verify-password")]
        //public async Task<IActionResult> VerifyPassword([FromBody] string username, [FromBody] Guid userId, [FromBody] string password)
        //{

        //}


    }   
}

