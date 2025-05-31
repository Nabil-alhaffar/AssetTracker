using System;
using System.Text.Json.Serialization;

namespace AssetTracker.Models.Finnhub
{
	public class FinancialsReportedResponse
	{
     

            [JsonPropertyName("data")]
            public List<FinancialReportFiling> Data { get; set; }
        
    }
}

