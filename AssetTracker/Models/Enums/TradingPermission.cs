namespace AssetTracker.Models.Enums
{
    /// <summary>
    /// Represents different trading permissions for users.
    /// </summary>
    public enum TradingPermission
    {
        /// <summary>
        /// Can trade stocks.
        /// </summary>
        Stocks,

        /// <summary>
        /// Can trade options.
        /// </summary>
        Options,

        /// <summary>
        /// Can trade futures.
        /// </summary>
        Futures,

        /// <summary>
        /// Can trade forex.
        /// </summary>
        Forex,

        /// <summary>
        /// Can trade cryptocurrencies.
        /// </summary>
        Cryptocurrencies,

        /// <summary>
        /// Can trade bonds.
        /// </summary>
        Bonds,

        /// <summary>
        /// Can trade ETFs.
        /// </summary>
        ETFs,

        /// <summary>
        /// Can trade mutual funds.
        /// </summary>
        MutualFunds,

        /// <summary>
        /// Can trade penny stocks.
        /// </summary>
        PennyStocks,

        /// <summary>
        /// Can trade on margin.
        /// </summary>
        MarginTrading,

        /// <summary>
        /// Can trade after hours.
        /// </summary>
        AfterHoursTrading,

        /// <summary>
        /// Can trade pre-market.
        /// </summary>
        PreMarketTrading,

        /// <summary>
        /// Can trade international securities.
        /// </summary>
        InternationalTrading,

        /// <summary>
        /// Can trade complex options strategies.
        /// </summary>
        ComplexOptions,

        /// <summary>
        /// Can trade naked options.
        /// </summary>
        NakedOptions,

        /// <summary>
        /// Can trade short positions.
        /// </summary>
        ShortSelling,

        /// <summary>
        /// Can trade leveraged ETFs.
        /// </summary>
        LeveragedETFs,

        /// <summary>
        /// Can trade inverse ETFs.
        /// </summary>
        InverseETFs
    }
} 