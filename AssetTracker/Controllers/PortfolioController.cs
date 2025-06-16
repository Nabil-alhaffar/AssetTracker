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
using AssetTracker.Models.Enums;
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

        /// <summary>
        /// Retrieves a summary of the user's portfolio including market value, cost, and PnL.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <returns>Portfolio summary or error message</returns>
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
                    totalNetValue = portfolioSummary.NetAccountValue,
                    openPnL = portfolioSummary.OpenPNL,
                    percentagePnL = portfolioSummary.OpenReturnPercentage,
                    dayPnL = portfolioSummary.DayPNL,
                    dayPercentagePnL = portfolioSummary.DayReturnPercentage,


                }) ;
            }
            catch (Exception err) {

                return NotFound(new { message = err.Message });

            }
        }

        /// <summary>
        /// Retrieves the user's full portfolio data.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <returns>Portfolio object or error message</returns>
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


        /// <summary>
        /// Retrieves portfolio performance over a specified number of days.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <param name="days">Number of days to evaluate performance</param>
        /// <returns>Performance metrics including PnL and percentage return</returns>
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


        /// <summary>
        /// Retrieves transaction history for a specific stock position.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <param name="symbol">Stock symbol</param>
        /// <returns>List of historical transactions</returns>
        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetPositionHistory(Guid userId, string symbol)
        {
            var history = await _positionService.GetPositionHistoryAsync(userId,symbol);

            if (history == null || !history.Any())
                return NotFound(new { message = "No transaction history found for this position." });

            return Ok(new { message = "Position history retrieved successfully.", history });
        }


        /// <summary>
        /// Deposits funds into a user's portfolio.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <param name="depositAmount">Amount to deposit</param>
        /// <returns>Success message or error</returns>
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

        /// <summary>
        /// Withdraws funds from a user's portfolio.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <param name="withdrawAmount">Amount to withdraw</param>
        /// <returns>Success message or error</returns>
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


        /// <summary>
        /// Retrieves a user's stock position for a specific symbol.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <param name="symbol">Stock symbol</param>
        /// <returns>Position object or error</returns>
        [HttpGet("Positions/{userId}/{symbol}")]
        public async Task<IActionResult> GetPositionBySymbol(Guid userId, string symbol)
        {
            try
            {
                var position = await _portfolioService.GetUserPositionBySymbol(userId, symbol);
                return Ok(position);
            }
            catch(Exception ex)
            {
                return NotFound(new {message = $"Error fetching position: {ex}" });
            }
        }

        /// <summary>
        /// Retrieves all stock positions in a user's portfolio.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <returns>List of positions or error</returns>
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

        /// <summary>
        /// Triggers an update for the user's portfolio.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <returns>Success message or error</returns>
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

    }
}

