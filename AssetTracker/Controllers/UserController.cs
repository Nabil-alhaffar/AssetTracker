    using System;
using AssetTracker.Models;
using AssetTracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssetTracker.Controllers
{
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _userService.AddUserAsync(user);
            return Ok("User created successfully");
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
    }   
}

