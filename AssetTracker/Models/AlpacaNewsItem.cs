using System;
namespace AssetTracker.Models
{
	public class AlpacaNewsItem
	{
        public int Id { get; set; }
        public string Author { get; set; }
        public string Headline { get; set; }
        public string Summary { get; set; }
        public string Url { get; set; }
        public string Source { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Symbols { get; set; }
    }
}

