using System;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AssetTracker.Helpers
{

    /// <summary>
    /// Helper methods for handling JWT tokens.
    /// </summary>
    public static class JwtHelpers
	{

        /// <summary>
        /// Validates a JWT token ignoring its expiration and extracts the <see cref="ClaimsPrincipal"/> from it.
        /// This is typically used to get user claims from an expired token during a refresh token process.
        /// </summary>
        /// <param name="token">The JWT token string to validate and parse.</param>
        /// <param name="configuration">The application configuration to get JWT settings like issuer, audience, and secret key.</param>
        /// <returns>A <see cref="ClaimsPrincipal"/> extracted from the token.</returns>
        /// <exception cref="SecurityTokenException">Thrown when the token is invalid or has an unexpected signing algorithm.</exception>
        public static ClaimsPrincipal GetPrincipalFromExpiredToken(string token, IConfiguration configuration)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false, // Ignore expiration
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }

}

