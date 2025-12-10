using System;
using System.ComponentModel.DataAnnotations;

namespace LegitimateAuthService.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string? FullName { get; set; }

        public string? Department { get; set; }

        public string Role { get; set; } = "Employee";

        public bool MfaEnabled { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLogin { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public ICollection<LoginLog> LoginLogs { get; set; } = new List<LoginLog>();
        public ICollection<Resource> Resources { get; set; } = new List<Resource>();
    }
}
