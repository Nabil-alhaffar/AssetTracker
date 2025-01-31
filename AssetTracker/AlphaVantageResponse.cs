using System;
namespace AssetTracker
{
	public class AlphaVantageResponse
	{
		public string Symbol { get; set; }
		public Dictionary<string,string> TimeSeries { get; set; }
		public AlphaVantageResponse()
		{
		}
	}
}

