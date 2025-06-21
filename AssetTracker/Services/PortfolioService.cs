using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetTracker.Models;
using AssetTracker.Repositories;
using AssetTracker.Repositories.Interfaces;
using AssetTracker.Services.Interfaces;

namespace AssetTracker.Services
{
    /// <summary>
    /// Provides portfolio-related operations such as retrieving portfolio details, positions, and updating funds.
    /// </summary>
    public class PortfolioService : IPortfolioService
    {
        private readonly IUserService _userService;
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IHistoricalPortfolioValueRepository _historicalPortfolioValueRepository;
        private readonly IPositionService _positionService;
        private readonly IAlphaVantageStockMarketService _alphaVantageStockMarketService;
        private readonly IFinnhubStockMarketService _finnhubStockMarketService;

        public PortfolioService(IHistoricalPortfolioValueRepository historicalPortfolioValueRepository,
                                IPortfolioRepository portfolioRepository,
                                IPositionService positionService,
                                IAlphaVantageStockMarketService alphaVantageStockMarketService,
                                IFinnhubStockMarketService finnhubStockMarketService,
                                IUserService userService)
        {

            _userService = userService;
            _portfolioRepository = portfolioRepository;
            _historicalPortfolioValueRepository = historicalPortfolioValueRepository;
            _positionService = positionService;
            //_alphaVantageStockMarketService = alphaVantageStockMarketService;
            _finnhubStockMarketService = finnhubStockMarketService;

        }


        /// <summary>
        /// Retrieves the portfolio for a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>The user's portfolio.</returns>
        public async Task<Portfolio> GetPortfolioAsync(Guid userId)
        {
            try
            {
                var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);

                return portfolio;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching portfolio: {ex.Message}");
            }
        }

        /// <summary>
        /// Returns a summary of the user's portfolio including market value, cost, cash balance, PNL, and returns.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A summary of the portfolio.</returns>
        public async Task<PortfolioSummary> GetPortfolioSummaryAsync(Guid userId)
        {
            try
            {
                var totalMarketValue = await GetCurrentMarketValue(userId);
                var totalCost = await GetTotalCost(userId);
                var cashBalance = await GetAvailableFundsAsync(userId);
                // Call performance function with days = 1 (default)
                var performance = await GetPortfolioPerformanceAsync(userId, 1);
                var openPnl = await GetOpenPNLAsync(userId);
                
                // Get the full portfolio to access margin data
                var portfolio = await GetPortfolioAsync(userId);

                // Store today's market value
                //await _historicalPortfolioValueRepository.StoreMarketValueAsync(userId, DateOnly.FromDateTime(DateTime.Now), totalMarketValue);

                return new PortfolioSummary
                {
                    MarketValue = totalMarketValue,
                    Cost = totalCost,
                    CashBalance = cashBalance,
                    NetAccountValue = (totalMarketValue + cashBalance),
                    OpenPNL = openPnl.PNLValue,
                    OpenReturnPercentage= openPnl.PNLPercentage,
                    DayPNL = performance.PNL,
                    DayReturnPercentage = performance.ReturnPercentage,
                    MarginUsed = portfolio.MarginUsed,
                    MarginLimit = portfolio.MarginLimit,
                    BuyingPower = portfolio.BuyingPower,
                    Equity = portfolio.Equity,
                    IsInMarginCall = portfolio.IsInMarginCall,
                    MaintenanceMarginRequirement = portfolio.MaintenanceMarginRequirement,
                    InitialMarginRequirement = portfolio.InitialMarginRequirement
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calculating portfolio summary: {ex.Message}");
            }
        }


        /// <summary>
        /// Calculates the open PnL (profit and loss) and return percentage for all open positions in the user's portfolio.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A PnL object containing the open PnL value and percentage.</returns>
        private async Task<PnL> GetOpenPNLAsync(Guid userId)
        {
            decimal openPNL = 0;
            decimal totalCostOfOpenPositions = 0;
            var openPositions = await GetPortfolioPositionsAsync(userId);
            foreach (var position in openPositions.Values )
            {
                var summary = await _positionService.GetPositionSummaryAsync(userId, position.Symbol);

                openPNL += summary.OpenPNL;


                totalCostOfOpenPositions += summary.TotalCost;
            }
            decimal openReturnPercentage = 0;
            if (totalCostOfOpenPositions != 0)
            {
                openReturnPercentage = (openPNL / Math.Abs(totalCostOfOpenPositions)) * 100;
            }
            return new PnL { PNLValue = openPNL, PNLPercentage = openReturnPercentage };
        }

        /// <summary>
        /// Gets the current total value of the user's portfolio including market value and available funds.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>Total value of the portfolio.</returns>
        public async Task<decimal> GetCurrentTotalValue(Guid userId)
        {
            try
            {
                decimal TotalMarketValue = await GetCurrentMarketValue(userId);
                decimal totalCashBalance = await  GetAvailableFundsAsync(userId);
                return TotalMarketValue + totalCashBalance;

            }
            catch (Exception ex)
            {
                throw new Exception($"Error calculating Portfolio total value: {ex.Message}");

            }

        }

        /// <summary>
        /// Calculates the performance of the portfolio over a number of days.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="days">The number of days in the past to compare performance.</param>
        /// <returns>Portfolio performance metrics.</returns>
        public async Task<PortfolioPerformance> GetPortfolioPerformanceAsync(Guid userId, int days)
        {
            try
            {
                var todayTotalValue = await GetCurrentTotalValue(userId);
                var pastTotalValue = await GetTotalValueDaysAgo(userId, days);
                //var todayMarketValue = await GetCurrentMarketValue(userId);
                //var pastMarketValue = await GetMarketValueDaysAgo(userId, days);

                //if (pastMarketValue == null)
                //{
                //    return new PortfolioPerformance
                //    {
                //        PNL = 0,
                //        ReturnPercentage = 0
                //    };
                //}
                if (pastTotalValue == null)
                {
                    return new PortfolioPerformance
                    {
                        PNL = 0,
                        ReturnPercentage = 0
                    };
                }
                //decimal pnl = todayMarketValue - pastMarketValue.Value;
                //decimal returnPercentage = pastMarketValue.Value > 0 ? (pnl / pastMarketValue.Value) * 100 : 0;
                decimal pnl = todayTotalValue - pastTotalValue.Value;

                decimal returnPercentage = pastTotalValue.Value > 0 ? (pnl / pastTotalValue.Value) * 100 : 0;

                return new PortfolioPerformance
                {
                    PNL = pnl,
                    ReturnPercentage = returnPercentage
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calculating portfolio performance: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves the total portfolio value from a specified number of days ago, falling back to the closest available value within the past week if not found.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="days">The number of days in the past.</param>
        /// <returns>The total portfolio value or null if unavailable.</returns>
        private async Task<decimal?> GetTotalValueDaysAgo(Guid userId, int days)
        {
            var user = await _userService.GetUserAsync(userId);

            // Default to UTC if no timezone is set
            var timeZoneId = string.IsNullOrEmpty(user.TimeZoneId) ? "UTC" : user.TimeZoneId;
            var userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

            var userLocalNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, userTimeZone);
            var pastDate = DateOnly.FromDateTime(userLocalNow.AddDays(-days));

            return await _historicalPortfolioValueRepository.GetTotalValueOnDateAsync(userId, pastDate)
                   ?? await GetClosestAvailableTotalValue(userId, pastDate);
        }

        /// <summary>
        /// Attempts to find the closest available total portfolio value for a user up to 7 days before the requested date.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="requestedDate">The date from which to begin the search.</param>
        /// <returns>The closest total portfolio value or null if none is found.</returns>
        private async Task<decimal?> GetClosestAvailableTotalValue(Guid userId, DateOnly requestedDate)
        {
            for (int i = 1; i <= 7; i++) // Try up to a week back
            {
                var adjustedDate = requestedDate.AddDays(-i);
                var value = await _historicalPortfolioValueRepository.GetTotalValueOnDateAsync(userId, adjustedDate);
                if (value.HasValue) return value;
            }

            return null; // No valid historical data found
        }

        /// <summary>
        /// Computes the total current market value of all positions in the user's portfolio.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>The total current market value.</returns>
        private async Task<decimal> GetCurrentMarketValue(Guid userId)
        {
            var positions = await _portfolioRepository.GetPositionsByUserId(userId);
            decimal totalMarketValue = 0;

            foreach (var position in positions)
            {
                var positionSummary = await _positionService.GetPositionSummaryAsync(userId, position.Key);
                totalMarketValue += positionSummary.MarketValue;
            }

            return totalMarketValue;
        }


        /// <summary>
        /// Stores the total market value of the user's portfolio for today.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="marketValue">The total market value to store.</param>
        /// <returns>A task representing the asynchronous operation.</returns
        public async Task StoreTotalValueAsync(Guid userId, decimal marketValue)
        {
            var user = await _userService.GetUserAsync(userId);
            var timeZoneId = string.IsNullOrEmpty(user.TimeZoneId) ? "UTC" : user.TimeZoneId;
            var userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

            var userLocalToday = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, userTimeZone));
            await _historicalPortfolioValueRepository.StoreTotalValueAsync(userId, userLocalToday, marketValue);
        }

        /// <summary>
        /// Calculates the total cost basis for all positions in the user's portfolio.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>The total cost of all positions.</returns
        private async Task<decimal> GetTotalCost(Guid userId)
        {
            var positions = await _portfolioRepository.GetPositionsByUserId(userId);
            decimal totalCost = 0;

            foreach (var position in positions)
            {
                var positionData = position.Value;
                totalCost += positionData.Quantity * positionData.AveragePurchasePrice;
            }

            return totalCost;
        }

        /// <summary>
        /// Gets a specific position by stock symbol for a user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="symbol">The stock symbol.</param>
        /// <returns>The position object.</returns>
        public async Task<Position> GetUserPositionBySymbol(Guid userId, string symbol)
        {
            ;
            try
            {
                var position = await _portfolioRepository.GetUserPositionBySymbol(userId, symbol);
                return position;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching position: ", ex);
            }

        }


        /// <summary>
        /// Updates the available funds in the user's portfolio (e.g., deposit or withdrawal).
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="additionalAmount">The amount to add (or subtract) from available funds.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateAvailableFundsAsync(Guid userId, decimal additionalAmount)
        {
            if (userId == null)
            {
                throw new ArgumentException(nameof(userId), "userId cannot be null.");
            }

            Portfolio portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
            if (portfolio.AvailableFunds + additionalAmount < 0)
            {
                throw new ArgumentException(nameof(portfolio.AvailableFunds), "Available funds cannot be negative.");
            }

            portfolio.AvailableFunds += additionalAmount;
            await _portfolioRepository.UpdatePortfolioAsync(portfolio);
        }

        /// <summary>
        /// Gets the available funds in the user's portfolio.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>The available cash balance.</returns>
        public async Task<decimal> GetAvailableFundsAsync(Guid userId)
        {
            if (userId == null)
            {
                throw new ArgumentException(nameof(userId), "userId cannot be null.");
            }

            var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
            return portfolio.AvailableFunds;
        }

        /// <summary>
        /// Retrieves all positions in the user's portfolio.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A dictionary of positions keyed by stock symbol.</returns>
        public async Task<Dictionary<string, Position>> GetPortfolioPositionsAsync(Guid userId)
        {
            if (userId == null)
                throw new ArgumentException(nameof(userId), "userId cannot be null.");

            //decimal totalMarketValue = await GetCurrentMarketValue(userId);
            var positions = await _portfolioRepository.GetPositionsByUserId(userId);
            //foreach (var position in positions.Values)
            //{
            //    // Fetch current price (replace with actual data fetching logic)
            //    //decimal currentPrice = await _alphaVantageStockMarketService.GetStockPriceAsync(position.Symbol);
            //    position.CurrentPrice = currentPrice;
            //    position.ComputePositionRatio(totalMarketValue);
            //}
            return positions;
        }

        /// <summary>
        /// Refreshes all user portfolios by updating prices and position ratios.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RefreshPortfolioForAllUsersAsync()
        {
            var users = await _portfolioRepository.GetAllUserIdsAsync(); 
            foreach(var userId in users)
            {
                await RefreshPortfolioByUserId(userId);
            }

        }

        /// <summary>
        /// Updates a portfolio in the MongoDb Repository. 
        /// </summary>
        /// <param name="portfolio">The updated portfolio object.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdatePortfolioAsync(Portfolio portfolio)
        {
            try
            {
                await _portfolioRepository.UpdatePortfolioAsync(portfolio);

            }
            catch(Exception ex)
            {
                throw new Exception("Update portfolio Failed:",ex);
            }

        }   



        /// <summary>
        /// Refreshes a specific user's portfolio prices and ratios based on the latest market data.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RefreshPortfolioByUserId(Guid userId)
        {
            try
            {
                decimal totalMarketValue = await GetCurrentMarketValue(userId);
                //var positions = await _portfolioRepository.GetPositionsByUserId(userId);
                var portfolio = await _portfolioRepository.GetUserPortfolioAsync(userId);
                foreach (var position in portfolio.Positions.Values)
                {
                    // Fetch current price (replace with actual data fetching logic)
                    var quote = await _finnhubStockMarketService.GetQuoteAsync(position.Symbol);
                    decimal currentPrice = quote.C;

                    //decimal currentPrice = await _alphaVantageStockMarketService.GetStockPriceAsync(position.Symbol);
                    position.CurrentPrice = currentPrice;
                    position.ComputePositionRatio(totalMarketValue);
                }
                await _portfolioRepository.UpdatePortfolioAsync(portfolio);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating {userId}'s Portfolio: {ex.Message}");
            }


        }
        /// <summary>
        /// Refreshes and stores the total portfolio values for all users for the current day.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RefreshTotalValuesForAllUsersAsync()
        {
            var users = await _portfolioRepository.GetAllUserIdsAsync(); 

            foreach (var userId in users)
            {
                //var marketValue = await GetCurrentMarketValue(userId);
                var totalValue = await GetCurrentTotalValue(userId);
                await StoreTotalValueAsync(userId, totalValue);
            }
        }

    }
}
