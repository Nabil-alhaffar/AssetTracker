using System;
namespace AssetTracker.Models
{

    public sealed record RegisterRequest
    {
        public string UserName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; }= null!;
        public string Email { get; set; } =null!;
        public string Password { get; set; }=  null!;

        public string? TimeZoneId { get; set; }

    }
}

