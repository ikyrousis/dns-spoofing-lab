using LegitimateAuthService.Services;
using Microsoft.AspNetCore.Mvc;

namespace LegitimateAuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IAuthService authService, ILogger<DashboardController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserData([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { success = false, message = "Email parameter is required" });
            }

            _logger.LogInformation("Dashboard data requested for {Email}", email);

            var data = await _authService.GetDashboardDataAsync(email);

            return Ok(data);
        }

        [HttpGet("resource/{id}")]
        public IActionResult GetResource(int id)
        {
            _logger.LogInformation("Resource {ResourceId} download requested", id);
            
            // In a real implementation, this would return the actual file
            // For this demo, we return a placeholder response
            return Ok(new { success = true, message = $"Resource {id} download would start here" });
        }

        [HttpGet("activity")]
        public IActionResult GetActivity()
        {
            _logger.LogInformation("Activity log requested");
            
            // Return mock activity data
            var activities = new[]
            {
                new
                {
                    id = 1,
                    action = "Login",
                    timestamp = DateTime.UtcNow.AddSeconds(-10).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
                }
            };

            return Ok(activities);
        }
    }
}
