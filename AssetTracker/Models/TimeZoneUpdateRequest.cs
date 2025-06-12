using System;
using System.ComponentModel.DataAnnotations;

namespace AssetTracker.Models
{

    public class TimeZoneUpdateRequest
    {
        [Required]
        [RegularExpression(@"^[A-Za-z/_]+$", ErrorMessage = "Invalid time zone format.")]
        public string TimeZoneId { get; set; }
    }
}


