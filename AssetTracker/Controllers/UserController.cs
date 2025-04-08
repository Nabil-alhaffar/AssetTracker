    using System;
using AssetTracker.Models;
using AssetTracker.Services;
using Microsoft.AspNetCore.Mvc;
using AssetTracker.Services.Interfaces;

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


        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(Guid userId)
        {
            var user = await _userService.GetUserAsync(userId);
            if (user == null) return NotFound("User not found");
            return Ok(user);
        }
        [HttpGet("Get-all")]
        public async Task <IActionResult> GetAllUsers()
        {
            var users = await _userService.GetUsersAsync();

            if (!users.Any())
                return NotFound(new { message = "No users found." });

            return Ok(new { count = users.Count(), users });

        }
        [HttpPatch("{userId}/reset-password")]
        public async Task <IActionResult> ResetPassword (Guid userId,[FromBody] ResetPasswordRequest request)
        {
            try
            {
                await _userService.ResetPassword(userId, request.NewPassword);

                return Ok("Password has been successfully reset");

            }
            catch (Exception ex)
            {
                return BadRequest($"Error resetting password: {ex.Message}");
            }
        }
        [HttpPatch("{userId}/reset-email")]
        public async Task<IActionResult> ResetEmail(Guid userId, [FromBody] ResetEmailRequest request)
        {
            try
            {
                await _userService.ResetEmail(userId, request.NewEmail);
                return Ok("Email has been successfully reset");

            }
            catch (Exception ex)
            {
                return BadRequest($"Error resetting email: {ex.Message}");
            }
        }
        [HttpPatch("{userId}/reset-username")]
        public async Task<IActionResult> ResetUsername(Guid userId, [FromBody] ResetUsernameRequest request)
        {
            try
            {
                await _userService.ResetUsername(userId, request.NewUsername);
                return Ok("Username has been successfully reset");

            }
            catch (Exception ex)
            {
                return BadRequest($"Error resetting username: {ex.Message}");
            }
        }
        //[HttpPost("verify-password")]
        //public async Task<IActionResult> VerifyPassword([FromBody] string username, [FromBody] Guid userId, [FromBody] string password)
        //{

        //}


    }   
}

