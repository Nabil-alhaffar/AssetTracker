using System;
namespace AssetTracker.Models
{
	public sealed record TradeResponse
	{
		public bool Success { get; set; }
		public string Message { get; set; }

		public TradeResponse(bool success, string message)
		{
			Success = success;
			Message = message;
		}
	}
}

