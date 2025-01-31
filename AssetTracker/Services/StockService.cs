using System;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace AssetTracker.Services
{
	public class StockService: IStockService
	{
		private readonly HttpClient _httpClient;
		private readonly string APIKey = "0QRZHQL1XJD2F1GR";
		private const string BaseURL = "https://www.alphavantage.co/query";
		public StockService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}
		public async  Task<double> GetStockPriceAsync (string symbol){

			var url = $"{BaseURL}?function=TIME_SERIES_INTRADAY&symbol={symbol}&interval=1min&apikey={APIKey}";
			var response = await _httpClient.GetStringAsync(url);
			var json = JObject.Parse(response);
			//var stockdata = JsonSerializer.Deserialize<AlphaVantageResponse>(response);
			var timeSeries = json["Time Series (1min)"];
			var latestTime = timeSeries?.First?.First;
			var closePrice = latestTime?["4. close"]?.ToString();

			return closePrice != null ? double.Parse(closePrice) : 0;
		}
	}
}

