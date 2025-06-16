using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace AssetTracker.Helpers
{
    /// <summary>
    /// Extension methods for <see cref="ClaimsPrincipal"/> to simplify claim retrieval.
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {

        /// <summary>
        /// Retrieves the user ID (NameIdentifier claim) from the <see cref="ClaimsPrincipal"/>.
        /// Throws <see cref="UnauthorizedAccessException"/> if the claim is missing or invalid.
        /// </summary>
        /// <param name="user">The claims principal representing the authenticated user.</param>
        /// <returns>The user ID as a <see cref="Guid"/>.</returns>
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var id) ? id : throw new UnauthorizedAccessException("Invalid or missing user ID.");
        }

    }
}