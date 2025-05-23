using System;
using System.Text.Json.Serialization;

namespace AssetTracker.Models
{
	public class AlpacaMostActiveResponse
	{

            [JsonPropertyName("last_updated")]
            public DateTime LastUpdated { get; set; }

            [JsonPropertyName("most_actives")]
            public List<AlpacaMostActiveItem> MostActives { get; set; }


    }
}

