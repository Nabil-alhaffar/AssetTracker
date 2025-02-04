﻿using System;
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
        public async Task<IActionResult> GetUser(int userId)
        {
            var user = await _userService.GetUserAsync(userId);
            if (user == null) return NotFound("User not found");
            return Ok(user);
        }
        [HttpGet("Get-all")]
        public async Task <IEnumerable<User>> GetAllUsers()
        {
            var users = await _userService.GetUsersAsync();
            return users;

        }
    }   
}

