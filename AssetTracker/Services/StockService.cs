using System;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using AssetTracker.Models;
using Newtonsoft.Json;

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
		//public async  Task<double> GetStockPriceAsync (string symbol){

		//	//var url = $"{BaseURL}?function=TIME_SERIES_INTRADAY&symbol={symbol}&interval=1min&apikey={APIKey}";
		//	//var response = await _httpClient.GetStringAsync(url);
		//	//var json = JObject.Parse(response);
		//	////var stockdata = JsonSerializer.Deserialize<AlphaVantageResponse>(response);
		//	//var timeSeries = json["Time Series (1min)"];
		//	//var latestTime = timeSeries?.First?.First;
		//	//var closePrice = latestTime?["4. close"]?.ToString();

		//	//return closePrice != null ? double.Parse(closePrice) : 0;
		//}

        public string GetCompanyLogoUrl(string website)
        {
            string domain = DomainExtractor(website);
            string logoUrl = $"https://logo.clearbit.com/{domain}";
            return logoUrl;
        }
        public async Task<Stock> GetStockOverviewAsync(string symbol)

        {
            
            

            try
            {
                var url = $"{BaseURL}?function=TIME_SERIES_INTRADAY&symbol={symbol}&interval=1min&apikey={APIKey}";
                var response = await _httpClient.GetStringAsync(url);
                var json = JObject.Parse(response);
                //var stockdata = JsonSerializer.Deserialize<AlphaVantageResponse>(response);
                var timeSeries = json["Time Series (1min)"];
                var latestTime = timeSeries?.First?.First;
                var currentPrice = (float)latestTime?["4. close"];
                string infoUrl = $"{BaseURL}?function=OVERVIEW&symbol={symbol}&apikey={APIKey}";
                HttpResponseMessage infoResponse = await _httpClient.GetAsync(infoUrl);
                infoResponse.EnsureSuccessStatusCode();

                string jsonResponse = await infoResponse.Content.ReadAsStringAsync();
                var stockInfo = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                
                var stock = new Stock
                {
                    Symbol = symbol,
                    CurrentPrice = currentPrice,
                    CompanyName = stockInfo["Name"] ?? symbol,
                    MarketCap = stockInfo["MarketCapitalization"] ?? 0,
                    EPS = stockInfo["EPS"] ?? 0,
                    StockStatus = (Stock.Status)(stockInfo["52WeekHigh"] > currentPrice ? 0 : 1), // Bullish or Bearish
                    StockSector = ParseSector(stockInfo["Sector"]?.ToString()),
                    Exchange = stockInfo["Exchange"]?.ToString() ?? "Unknown",
                    High52Week = stockInfo["52WeekHigh"] ?? 0,
                    Low52Week = stockInfo["52WeekLow"] ?? 0,
                    MovingAverage50Day = stockInfo["50DayMovingAverage"] ?? 0,
                    MovingAverage200Day = stockInfo["200DayMovingAverage"] ?? 0,
                    Country = stockInfo["Country"] ?? "Unknown",
                    DividendDate = stockInfo["DividendDate"] ?? DateTimeZoneHandling.Local,
                    ExDividendDate = stockInfo["ExDividendDate"] ?? DateTimeZoneHandling.Local,
                    AnalystTargetPrice = stockInfo["AnalystTargetPrice"]??"Unknown",
                    Description = stockInfo["Description"]?? "Unknown",
                    OfficialSite = stockInfo["OfficialSite"]?? "Unknown",
                    
                };

                return stock;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching stock overview: {ex.Message}");
                return null;
            }
        }
        private string DomainExtractor(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("URL cannot be null or empty", nameof(url));
            }

            // Ensure the URL has a scheme (http:// or https://)
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                // Prepend "http://" if the URL is missing a protocol
                if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                    !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    url = "http://" + url;
                }
            }

            try
            {
                Uri uri = new Uri(url);
                string host = uri.Host;

                // Remove "www." prefix if it exists
                if (host.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
                {
                    host = host.Substring(4);
                }

                return host;
            }
            catch (UriFormatException ex)
            {
                throw new UriFormatException($"The provided URL '{url}' is not valid. Exception: {ex.Message}");
            }
        }



        private Stock.Sector ParseSector(string sector)
        {
            return sector?.ToLower() switch
            {
                "energy" => Stock.Sector.Energy,
                "materials" => Stock.Sector.Materials,
                "industrials" => Stock.Sector.Industrials,
                "consumer discretionary" => Stock.Sector.ConsumerDiscretionary,
                "consumer staples" => Stock.Sector.ConsumerStaples,
                "healthcare" => Stock.Sector.Healthcare,
                "financials" => Stock.Sector.Financials,
                "information technology" => Stock.Sector.InformationTechnology,
                "communication services" => Stock.Sector.CommunicationServices,
                "utilities" => Stock.Sector.Utilities,
                "real estate" => Stock.Sector.RealEstate,
                _ => Stock.Sector.InformationTechnology // Default to Information Technology
            };
        }
    }
}

