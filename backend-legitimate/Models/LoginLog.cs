using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegitimateAuthService.Models
{
    public class LoginLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public string? IpAddress { get; set; }

        public string? UserAgent { get; set; }

        public bool Success { get; set; }

        public string? FailureReason { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }
}
