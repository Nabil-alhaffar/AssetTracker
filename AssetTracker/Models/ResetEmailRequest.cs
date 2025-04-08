using System;
namespace AssetTracker.Models
{
    public sealed record ResetEmailRequest
    {
        public string NewEmail { get; set; }
    }

}

