namespace CredentialCaptureService.Services;

using CredentialCaptureService.Models;

/**
 * Interface for credential storage operations
 */
public interface ICredentialService
{
    Task SaveCredentialAsync(CapturedCredential credential);
    Task<List<CapturedCredential>> GetAllCredentialsAsync();
}
