using System;
namespace AssetTracker.Models
{
    public sealed record ResetUsernameRequest
    {
        public string NewUsername { get; set; }
    }
}

