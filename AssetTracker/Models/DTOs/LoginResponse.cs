using System;
namespace AssetTracker.Models.DTOs
{
	public class LoginResponse
	{

        public Guid UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? TimeZoneId { get; set; }
        public string Token { get; set; } = null!;
        public string SessionId { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}

