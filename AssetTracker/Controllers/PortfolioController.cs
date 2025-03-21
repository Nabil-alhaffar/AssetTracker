using System;
using Microsoft.AspNetCore.Mvc;
using AssetTracker.Services;
using System.Threading.Tasks;
using AssetTracker.Models;
using AssetTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace AssetTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioService _portfolioService;
        private readonly IUserService _userService;
        private readonly IPositionService _positionService;
        private readonly ICashFlowLogService _cashFlowLogService;
        // Constructor with Dependency Injection
        public PortfolioController(IPortfolioService portfolioService, IUserService userService, IPositionService positionService, ICashFlowLogService cashFlowLogService)
        {
            _portfolioService = portfolioService;
            _userService = userService;
            _positionService = positionService;
            _cashFlowLogService = cashFlowLogService;
        }

        [HttpGet("summary/{userId}")]
        public async Task<IActionResult> GetPortfolioSummary(Guid userId)
        {
            try
            {
                var portfolioSummary = await _portfolioService.GetPortfolioSummaryAsync(userId);
                return Ok(new
                {
                    message = "Portfolio summary retrieved.",
                    totalMarketValue = portfolioSummary.MarketValue,
                    totalCost = portfolioSummary.Cost,
                    OpenPnL = portfolioSummary.OpenPNL,
                    PercentagePnL = portfolioSummary.OpenReturnPercentage,
                    TotalReturns = portfolioSummary.DayPNL,
                    returnPercentage = portfolioSummary.DayReturnPercentage
                }) ;
            }
            catch (Exception err) {

                return NotFound(new { message = err.Message });

            }
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserPortfolio(Guid userId)
        {
            try
            {
                var portfolio = await _portfolioService.GetUserPortfolioAsync(userId);
                return Ok(new
                {
                    portfolio
                }) ;
            }
            catch (Exception err)
            {

                return NotFound(new { message = err.Message });

            }
        }

        [HttpGet("performance/{userId}")]
        public async Task<IActionResult> GetPortfolioPerformance(Guid userId, int days)
        {
            try
            {
                var portfolioPerformance = await _portfolioService.GetPortfolioPerformanceAsync(userId, days);
                return Ok(new
                {
                    message = "Portfolio performance retrieved.",
                    pnl = portfolioPerformance.PNL,
                    percentagePNL = portfolioPerformance.ReturnPercentage

                }) ;
            }
            catch (Exception err)
            {

                return NotFound(new { message = err.Message });

            }
        }

        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetPositionHistory(Guid userId, string symbol)
        {
            var history = await _positionService.GetPositionHistoryAsync(userId,symbol);

            if (history == null || !history.Any())
                return NotFound(new { message = "No transaction history found for this position." });

            return Ok(new { message = "Position history retrieved successfully.", history });
        }

        [HttpPost("deposit-funds/{userId}")]
        [Authorize]
        public async Task <IActionResult> DepositFunds(Guid userId, decimal depositAmount)
        {
            try
            {
                await _portfolioService.UpdateAvailableFundsAsync(userId, depositAmount);

                CashFlowLog log = new CashFlowLog
                {
                    UserId = userId,
                    Amount = depositAmount,
                    Type = TransactionType.Deposit,
                    Description = $" Deposit inititated by {userId}: Amount: {depositAmount}" 
            
                };
                await _cashFlowLogService.AddLogAsync(log);
                return Ok(new { message = $"{depositAmount} was deposited successfully into {userId}'s account." });

            }
            catch
            {
                return NotFound(new { message = "Failed to add funds." });

            }
        }

        [HttpPost("withdraw-funds/{userId}")]
        [Authorize]
        public async Task<IActionResult> WithdrawFunds(Guid userId, decimal withdrawAmount)
        {
            try
            {
                await _portfolioService.UpdateAvailableFundsAsync(userId, -withdrawAmount);
                CashFlowLog log = new CashFlowLog
                {
                    UserId = userId,
                    Amount = withdrawAmount,
                    Type = TransactionType.Withdrawal,
                    Description = $" Withdrawal initiated by {userId}: Amount: {withdrawAmount}"

                };
                await _cashFlowLogService.AddLogAsync(log);
                return Ok(new { message = $"{withdrawAmount} was withdrawn successfully from {userId}'s account." });

            }
            catch
            {
                return NotFound(new { message = "Failed to Withdraw funds." });

            }
        }
        [HttpGet ("Positions/{userId}")]
        public async Task<IActionResult> GetPortfolioPositionsByUserId(Guid userId)
        {
            try
            {
              var positions =   await _portfolioService.GetPortfolioPositionsAsync(userId);
                return Ok(new { message = "Positions retrieved successfully.", positions });

            }
            catch
            {
                return NotFound(new { message = "Portfolio not found." });

            }
        }

        [HttpPost("UpdatePortfolio/{userId}")]
        public async Task<IActionResult> UpdatePortfolioByUserId(Guid userId)
        {
            try
            {
                await _portfolioService.UpdatePortfolioByUserId(userId);
                return Ok(new { message = "Portfolio updated successfully." });
            }
            catch
            {
                return NotFound(new { message = "Portfolio not found." });
            }
        }



        //// Add a new position to the portfolio
        //[HttpPost("add-position/{userId}")]
        //public async Task<ActionResult> AddPositionToPortfolio([FromBody] Position position,Guid userId)
        //{

        //    //Portfolio portfolio1 = await _portfolioService.GetPortfolioAsync(userId);
        //    User user = await _userService.GetUserAsync(userId);
        //    //if (portfolio1 == null)
        //    //{
        //    //    return NotFound($"cannot find portfolio");
        //    //}
        //    if(user == null) {
        //        return NotFound("User not found");
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //       return BadRequest(ModelState);
        //    }  
        //    var portfolio = user.Portfolio;
        //    if (portfolio == null)
        //    {
        //        return NotFound("Portfolio not found.");
        //    }
        //    position.PortfolioId = portfolio.PortfolioId;
        //    position.UserId = userId;
        //    try
        //    {
        //        await _portfolioService.AddPositionToPortfolioAsync(position, userId);
        //        return Ok("Position added or updated successfully.");

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Error: {ex.Message}"); // Handle any unexpected errors

        //    }



        // Add the position to the portfolio

        //user.Portfolio.Positions.Add(position);

        //    return Ok("Position added successfully.");
        //    //try
        //    //{
        //    //    await _portfolioService.AddPositionToPortfolioAsync(position,userId);
        //    //    return Ok("Position added successfully");
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    return BadRequest(ex.Message); // Handle errors gracefully
        //    //}
        //}

        //[HttpDelete("remove-position/{symbol}")]
        //public async Task<ActionResult> RemovePositionAsync(Guid userId, string symbol)
        //{
        //    try
        //    {
        //        await _portfolioService.RemovePositionAsync(userId ,symbol);
        //        return Ok("Position removed successfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message); // Handle errors gracefully
        //    }
        //}
        //[HttpGet("all")]



        //public async Task<ActionResult> GetAllPositionsAsync(Guid userId)
        //{
        //    try
        //    {
        //        var positions = await _portfolioService.GetAllPositionsAsync(userId);
        //        return Ok(positions);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message); // Handle errors gracefully
        //    }
        //}

        //[HttpGet("{userId}")]
        //public async Task<IActionResult> GetPortfolioAsync(int userId)
        //{
        //    var portfolio = await _portfolioService.GetUserPortfolioAsync(userId);

        //    if (portfolio == null)
        //        return NotFound("Portfolio not found for the given user.");

        //    return Ok(portfolio);
        //}
        //// Get the total portfolio value
        //[HttpGet("total-value/{userId}")]
        //public async Task<ActionResult<double>> GetTotalValue(Guid userId)
        //{
        //    try
        //    {
        //        var totalValue = await _portfolioService.GetTotalValueAsync(userId);
        //        return Ok(new { userId, totalValue });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }

        //}

        //// Get the total Profit & Loss
        //[HttpGet("total-pnl/{userId}")]
        //public async Task<ActionResult<double>> GetTotalProfitAndLoss(Guid userId)
        //{
        //    try
        //    {
        //        var totalProfitLoss = await _portfolioService.GetTotalProfitAndLossAsync( userId);
        //        return Ok(totalProfitLoss);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message); // Handle errors gracefully
        //    };
        //}
    }
}

