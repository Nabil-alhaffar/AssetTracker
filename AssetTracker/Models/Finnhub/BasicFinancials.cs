using System;
namespace AssetTracker.Models.Finnhub
{
	public class BasicFinancials
	{
        public string Symbol { get; set; }
        public Dictionary<string, object> Metric { get; set; }
    }
}

