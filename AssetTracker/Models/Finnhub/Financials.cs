using System;
namespace AssetTracker.Models.Finnhub
{
    public class Financials
    {
        public string Symbol { get; set; }
        public string StatementType { get; set; }
        public string Frequency { get; set; }
        public List<Dictionary<string, string>> Data { get; set; }
    }
}

