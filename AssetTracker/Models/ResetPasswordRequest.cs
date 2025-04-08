using System;
namespace AssetTracker.Models
{
    public sealed record ResetPasswordRequest
    {
        public string NewPassword { get; set; }
    }
}

