﻿using System;
using Microsoft.AspNetCore.Mvc;
using AssetTracker.Services;
using System.Threading.Tasks;
using AssetTracker.Models;

namespace AssetTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioService _portfolioService;
        private readonly IUserService _userService;

        // Constructor with Dependency Injection
        public PortfolioController(IPortfolioService portfolioService, IUserService userService)
        {
            _portfolioService = portfolioService;
            _userService = userService;
        }

        // Get the total portfolio value
        [HttpGet("total-value/{userId}")]
        public async Task<ActionResult<double>> GetTotalValue(int userId)
        {
            try
            {
                var totalValue = await _portfolioService.GetTotalValueAsync(userId);
                return Ok(totalValue);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Handle errors gracefully
            }
        }

        // Get the total Profit & Loss
        [HttpGet("total-pnl/{userId}")]
        public async Task<ActionResult<double>> GetTotalProfitAndLoss(int userId)
        {
            try
            {
                var totalProfitLoss = await _portfolioService.GetTotalProfitAndLossAsync( userId);
                return Ok(totalProfitLoss);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Handle errors gracefully
            };
        }

        // Add a new position to the portfolio
        [HttpPost("add-position/{userId}")]
        public async Task<ActionResult> AddPositionToPortfolio([FromBody] Position position,int userId)
        {
           
            //Portfolio portfolio1 = await _portfolioService.GetPortfolioAsync(userId);
            User user = await _userService.GetUserAsync(userId);
            //if (portfolio1 == null)
            //{
            //    return NotFound($"cannot find portfolio");
            //}
            if(user == null) {
                return NotFound("User not found");
            }

            if (!ModelState.IsValid)
            {
               return BadRequest(ModelState);
            }  
            var portfolio = user.Portfolio;
            if (portfolio == null)
            {
                return NotFound("Portfolio not found.");
            }
            position.PortfolioId = portfolio.Id;

           await _portfolioService.AddPositionToPortfolioAsync(position, userId);
            // Add the position to the portfolio

            //user.Portfolio.Positions.Add(position);

            return Ok("Position added successfully.");
            //try
            //{
            //    await _portfolioService.AddPositionToPortfolioAsync(position,userId);
            //    return Ok("Position added successfully");
            //}
            //catch (Exception ex)
            //{
            //    return BadRequest(ex.Message); // Handle errors gracefully
            //}
        }

        [HttpDelete("remove-position/{symbol}")]
        public async Task<ActionResult> RemovePositionAsync(int userId, string symbol)
        {
            try
            {
                await _portfolioService.RemovePositionAsync(userId ,symbol);
                return Ok("Position removed successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Handle errors gracefully
            }
        }
        [HttpGet("all")]
        public async Task<ActionResult> GetAllPositionsAsync(int userId)
        {
            try
            {
                var positions = await _portfolioService.GetAllPositionsAsync(userId);
                return Ok(positions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Handle errors gracefully
            }
        }

        //[HttpGet("{userId}")]
        //public async Task<IActionResult> GetPortfolioAsync(int userId)
        //{
        //    var portfolio = await _portfolioService.GetUserPortfolioAsync(userId);

        //    if (portfolio == null)
        //        return NotFound("Portfolio not found for the given user.");

        //    return Ok(portfolio);
        //}
    }
}

