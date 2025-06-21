# AssetTracker

## Overview

**AssetTracker** is a full-featured, scalable, and extensible **stock trading and portfolio management API** built with ASP.NET Core. It provides individual users with a robust backend to:

- Register and authenticate securely with JWT.
- Deposit and withdraw virtual funds.
- Execute **long and short trades** with real-time updates.
- Monitor real-time and historical portfolio **performance and valuation**.
- Access **real-time and historical market data**, indicators, and overviews.
- Set up and manage watchlists and alerts.
- Subscribe to **live price updates** via WebSockets (Alpaca).
- Schedule background tasks using Hangfire.
- Cache data efficiently using Redis.
 

This API is the foundation for a future cross-platform frontend (web & mobile) to offer a complete trading experience.

---

## Tech Stack

### 🖥 Backend
- **ASP.NET Core 7 (C#)**
- **MongoDB** (NoSQL database)
- **Redis** (caching layer)
- **Hangfire** (background job scheduling)
- **SignalR** (real-time communication)

### 📡 APIs
- **Alpaca** – real-time & historical stock data, WebSockets
- **Alpha Vantage** – technical indicators, historical data
- **FinnHub **-- Company profiles and financial and earnings data 
### 🧰 DevOps & Infrastructure
- **AWS EC2** – cloud hosting
- **Docker** – containerization
- **AWS Secrets Manager** – secure secret storage

### 🎯 Frontend (Currently in development)
- Expo React Native (Mobile & Web)

---

## API Usage Examples:

### Authentication
    •   POST /api/auth/register --> Registers a new user and creates a new virtual profile. 
    •   POST /api/auth/login   --> Logins user in and starts a new session when authenticated, returning a JWT access token
    •   POST /api/auth/refresh --> refreshes JWT access token a valid refresh token is included as HTTP only cookie.

### Portfolio Management
    •   GET /api/portfolio/{userId} --> Retrieves the user's portfolio.
    •   GET /api/portfolio/performance/{userId}?days={days} --> Returns the portfolio performance over the given period.
    •   GET /api/portfolio/summary/{userId} --> Returns portfolio summary including margin information.
    •   POST /api/portfolio/margin/limit/{userId} --> Sets the margin limit for a user's portfolio.
    •   GET /api/portfolio/margin/status/{userId} --> Gets current margin status for a user's portfolio.
    •   POST /api/portfolio/margin/resolve/{userId} --> Adds funds to resolve a margin call.
    
### Market Data
    •   GET /api/AlphaVantageStockMarketController/getPrice/{symbol} --> Fetches current stock price.
    •   GET /api/AlphaVantageStockMarketController/indicators --> Fetches a list of timestamped technical indicators (EMA, SMA, BBANDS, RSI) based on the selected time period and interval.
    •   POST /api/SymbolSubscriptionController/{userId}/subscribe-to-Symbol/{symbol} --> Subscribes user to live updates for a symbol via alpaca WebSocket.
    •   GET /api/FinnHubController/profile/{symbol} --> Retrieves finnhub's financial profile for a ticker. 
    •   GET /api/alpaca/snapshots/ --> Retrieves ticker's latest cumulative snapshop including price, bar, 
    •   GET /api/alpaca/{symbol}/historicaldata/{timeframe}  --> Retrieves a stock's historical bars (OCHLV) based on the provided timeframe. Example: 1Day retrieves daily bars while 5min retrieves the 5min bars and so on. 
    
### US. Equity (Virtual) Trade Execution
    •   POST /api/StockController/execute-trade --> Executes a paper trading order based on fetched real time price data, supporting both long and short trades, and correspondingly updating user positions and portfolios. 


### Cash Flow Log Operations
    •   GET /api/CashFlowLogController/{userId} --> Retrieves all cash flow logs for a specific user.
    •   GET /api/CashFlowLogController/{transactionId} --> Retrieves a specific cash flow log by transaction ID.
    •   POST /api/CashFlowLogController/create --> Creates a new cash flow log.

### Position Operations
    •   PUT /api/PositionController/{userId}/split/{symbol} --> performs a split (or reverse split) on a user's position based on a split factor due to a corresponding corporate action.
    •   GET /api/PositionController/{userId}/check-stoploss/{symbol} --> Checks and triggers a stop loss order if an a user's stop loss price was reached. 

### Watchlist Operations
    •   GET /api/WatchlistController/{userId} --> Retrieves all user's watchlists. 
    •   POST /api/WatchlistController/{userId} --> Allows a user to add a new watchlist. 
    •   POST /api/WatchlistController/{userId}/{watchlistId}/add-symbol --> Allows a user to add a new symbol to an existing watchlist. 
    
### Margin Call Monitoring
    •   GET /api/alert/margin-calls --> Checks for margin calls across all portfolios.
    •   GET /api/alert/margin-calls/{userId} --> Gets margin call status for a specific user.
    •   GET /api/alert/margin-calls/my-status --> Gets margin call status for the authenticated user.

## Installation & Setup

### Prerequisites

Ensure you have the following installed:

- [.NET SDK](https://dotnet.microsoft.com/download)
- [MongoDB](https://www.mongodb.com/try/download/community)
- [Redis](https://redis.io/download)
- [Docker](https://www.docker.com/) (optional, for containerization)

### Steps to Run Locally

```bash
# Clone the repository
git clone https://github.com/Nabil-alhaffar/AssetTracker
cd AssetTracker

# Restore dependencies
dotnet restore

# Configure environment variables:
# - API keys for Alpaca, Alpha Vantage, and Finnhub. 
# - MongoDB and Redis connection strings
# - Hangfire Password
# - HTTPS certificate
# - JWT secret
# - AWS credentials (if using Secrets Manager)

# Run the application
dotnet run
