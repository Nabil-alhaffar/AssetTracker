#AssetTracker

    ##Overview:

    AssetTracker is a web API project that enables users to:

        - Register and log in.

        - Deposit and withdraw money.

        - Execute long and short trades.

        - Set up alerts.

        - View portfolio and position summary and performance.

        - Access historical and real-time stock data and indicators.

        - Manage watchlists.
        
        

    ##Technologies Used:

        ###Backend:

            - ASP.NET Core (C#) 

            - MongoDB

            - Redis

            - Hangfire (for scheduling)
            

        ###APIs:

            - Alpha Vantage

            - Alpaca
            
            

        ###Other Tools:

            - Docker

            - AWS EC2
            
            

        ###Frontend (Future Considerations):

            React.js, Angular, and/or React Native  (to be decided)
            
            

    ##Installation & Setup:
    

        ###Prerequisites:

            - .NET SDK

            - MongoDB

            - Redis

            - Docker (if running in containers)

        ###Steps to Clone and Run Locally:

            - Clone the repository: 
                - git clone https://github.com/Nabil-alhaffar/AssetTracker
                - cd AssetTracker

            - Install dependencies and required tools.

            - Configure environment variables in appsettings.json or .env (API keys, AWS secrets, DB connections).

            -Run the application: dotnet run
            
            


    ##Usage:

        AssetTracker exposes a RESTful API with the following being some of the key endpoints:

        ### Portfolio Management: 
            -GET /api/portfolio/{userId} - Retrieves the portfolio for a given user.
            -GET /api/portfolio/performance/{userId}?days={days}- Fetches the performance summary for a user's portfolio over a specified number of days.

        ###Watchlist:

            - POST /api/watchlist/{userId} - Adds a new watchlist for a user.

        ### Stock Data:

            - GET /api/stock/getPrice/{symbol} - Gets the current stock price for a given symbol.
            - POST /api/stock/subscribe/{symbol} - Subscribes to real-time updates for a specific stock.
            
            

    ##Features in Development:

         Planned enhancements include:

            - Trading bot.

            - AI-driven position evaluator and signal alerts.

            - Frontend (mobile/web app)
    
            - Broker integration
        
        

    ##Deployment

         AssetTracker is deployed using:

        - AWS EC2 for hosting the backend.

        - Docker for containerization and deployment.

        - Hangfire for job scheduling and background tasks.

        - Future enhancements may include:

        - CI/CD pipelines for automated deployment.

        - Kubernetes for scaling.
