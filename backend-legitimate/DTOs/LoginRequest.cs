using System.ComponentModel.DataAnnotations;

namespace LegitimateAuthService.DTOs
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string? Timestamp { get; set; }

        public string? UserAgent { get; set; }
    }
}
