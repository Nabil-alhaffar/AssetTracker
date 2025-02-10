using System;
namespace AssetTracker.Models
{
	public class TradeResult
	{
		public bool Success { get; set; }
		public string Message { get; set; }

		public TradeResult(bool success, string message)
		{
			Success = success;
			Message = message;
		}
	}
}

