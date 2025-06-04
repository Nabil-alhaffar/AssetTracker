using System;
using System.Security.Claims;

namespace AssetTracker.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var id) ? id : throw new UnauthorizedAccessException("Invalid or missing user ID.");
        }
    }
}

