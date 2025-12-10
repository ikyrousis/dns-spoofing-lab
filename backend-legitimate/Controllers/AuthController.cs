using LegitimateAuthService.DTOs;
using LegitimateAuthService.Services;
using Microsoft.AspNetCore.Mvc;

namespace LegitimateAuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", service = "LegitimateAuthService", timestamp = DateTime.UtcNow });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid request data" });
            }

            // Get client IP address and convert from IPv6 to IPv4 if needed
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (ipAddress != null && ipAddress.StartsWith("::ffff:"))
            {
                ipAddress = ipAddress.Substring(7); // Remove "::ffff:" prefix
            }

            _logger.LogInformation("Login attempt for {Email} from {IP}", request.Email, ipAddress);

            var result = await _authService.AuthenticateAsync(request, ipAddress);

            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            _logger.LogInformation("Logout request");
            return Ok(new { success = true, message = "Logged out successfully" });
        }
    }
}
