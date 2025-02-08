using System;
using AssetTracker.Models;
using Newtonsoft.Json;

namespace AssetTracker.Services
{
    public class AlertService:IAlertService
    {
        private readonly List<StockAlert> _alerts = new();
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string BaseUrl = "https://www.alphavantage.co/query";

        public AlertService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["AlphaVantage:ApiKey"]; // Fetch API Key from appsettings.json
        }

        public void AddAlert(StockAlert alert)
        {
            _alerts.Add(alert);
        }

        public void RemoveAlert(string symbol, AlertCondition condition)
        {
            _alerts.RemoveAll(a => a.Symbol == symbol && a.Condition.Type == condition.Type);
        }

        public IEnumerable<StockAlert> GetAlerts()
        {
            return _alerts;
        }

        public async Task<IEnumerable<StockAlert>> CheckAlertsAsync()
        {
            var triggeredAlerts = new List<StockAlert>();
            foreach (var alert in _alerts)
            {
                string function = alert.Condition.Type switch
                {
                    AlertType.SMA => "SMA",
                    AlertType.EMA => "EMA",
                    AlertType.MACD => "MACD",
                    AlertType.RSI => "RSI",
                    AlertType.BollingerBands => "BBANDS",
                    _ => "TIME_SERIES_INTRADAY"
                };

                var url = function == "TIME_SERIES_INTRADAY"
                    ? $"{BaseUrl}?function=TIME_SERIES_INTRADAY&symbol={alert.Symbol}&interval=1min&apikey={_apiKey}"
                    : $"{BaseUrl}?function={function}&symbol={alert.Symbol}&interval=daily&time_period=14&apikey={_apiKey}";

                var response = await _httpClient.GetStringAsync(url);
                var data = JsonConvert.DeserializeObject<AlphaVantageIndicatorResponse>(response);

                decimal? value = function switch
                {
                    "TIME_SERIES_INTRADAY" => data?.TimeSeries?.Values.FirstOrDefault()?.Close,
                    "BBANDS" => data?.BBANDS?.Values.FirstOrDefault()?.MiddleBand, // Use Middle Band for alerts
                    _ => data?.IndicatorValue
                };

                if (value.HasValue && alert.IsTriggered(value.Value))
                {
                    alert.TriggerAction();
                    triggeredAlerts.Add(alert);
                }
            }
            return triggeredAlerts ;
        }
    }

    

    

    

}

