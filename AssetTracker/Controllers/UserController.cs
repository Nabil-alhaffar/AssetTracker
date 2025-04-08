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

    }   
}

