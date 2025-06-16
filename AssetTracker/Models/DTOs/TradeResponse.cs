using System;

namespace AssetTracker.Models
{
    /// <summary>
    /// Represents the response returned after attempting a trade.
    /// </summary>
    public sealed record TradeResponse
    {
        /// <summary>
        /// Indicates whether the trade was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Provides a message describing the result of the trade.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeResponse"/> record.
        /// </summary>
        /// <param name="success">True if trade succeeded; otherwise, false.</param>
        /// <param name="message">Descriptive message about the trade result.</param>
        public TradeResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}