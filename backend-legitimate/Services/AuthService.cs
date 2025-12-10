using LegitimateAuthService.Data;
using LegitimateAuthService.Models;
using LegitimateAuthService.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LegitimateAuthService.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordService _passwordService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            AppDbContext context,
            IPasswordService passwordService,
            ILogger<AuthService> logger)
        {
            _context = context;
            _passwordService = passwordService;
            _logger = logger;
        }

        public async Task<LoginResponse> AuthenticateAsync(LoginRequest request, string? ipAddress)
        {
            try
            {
                // Find user by email
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

                if (user == null)
                {
                    // Log failed attempt (user not found)
                    _logger.LogWarning("Login attempt for non-existent user: {Email}", request.Email);
                    
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid email or password"
                    };
                }

                // Verify password
                if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash))
                {
                    // Log failed attempt (wrong password)
                    await LogLoginAttemptAsync(user.Id, ipAddress, request.UserAgent, false, "Invalid password");
                    
                    _logger.LogWarning("Failed login attempt for user: {Email}", request.Email);
                    
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid email or password"
                    };
                }

                // Successful authentication
                // Update last login
                user.LastLogin = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Log successful attempt
                await LogLoginAttemptAsync(user.Id, ipAddress, request.UserAgent, true, null);

                _logger.LogInformation("Successful login for user: {Email}", request.Email);

                return new LoginResponse
                {
                    Success = true,
                    Message = "Authentication successful",
                    User = new UserResponse
                    {
                        Email = user.Email,
                        FullName = user.FullName ?? "User",
                        Role = user.Role
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during authentication for {Email}", request.Email);
                return new LoginResponse
                {
                    Success = false,
                    Message = "An error occurred during authentication"
                };
            }
        }

        public async Task<DashboardResponse> GetDashboardDataAsync(string email)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Resources)
                    .Include(u => u.LoginLogs)
                    .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

                if (user == null)
                {
                    _logger.LogWarning("Dashboard data requested for non-existent user: {Email}", email);
                    return new DashboardResponse();
                }

                var response = new DashboardResponse
                {
                    User = new DashboardUserInfo
                    {
                        Email = user.Email,
                        FullName = user.FullName ?? "User",
                        Department = user.Department ?? "N/A",
                        Role = user.Role,
                        LastLogin = user.LastLogin?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") ?? DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                    },
                    Resources = user.Resources.Select(r => new ResourceResponse
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Type = r.Type,
                        Size = r.Size,
                        CreatedAt = r.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                    }).ToList(),
                    Announcements = GetAnnouncements(),
                    ActivityLogs = user.LoginLogs
                        .OrderByDescending(l => l.Timestamp)
                        .Take(10)
                        .Select(l => new ActivityLogResponse
                        {
                            Action = "Login",
                            IpAddress = l.IpAddress ?? "Unknown",
                            Success = l.Success,
                            Timestamp = l.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                        }).ToList()
                };

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dashboard data for {Email}", email);
                return new DashboardResponse();
            }
        }

        private async Task LogLoginAttemptAsync(int userId, string? ipAddress, string? userAgent, bool success, string? failureReason)
        {
            try
            {
                var log = new LoginLog
                {
                    UserId = userId,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    Success = success,
                    FailureReason = failureReason,
                    Timestamp = DateTime.UtcNow
                };

                _context.LoginLogs.Add(log);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging login attempt for user {UserId}", userId);
            }
        }

        private List<AnnouncementResponse> GetAnnouncements()
        {
            // Static announcements (could be moved to database in future)
            return new List<AnnouncementResponse>
            {
                new AnnouncementResponse
                {
                    Id = 1,
                    Title = "System Maintenance Scheduled",
                    Message = "Planned maintenance window on Dec 15, 2025 from 2-4 AM EST.",
                    Date = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                },
                new AnnouncementResponse
                {
                    Id = 2,
                    Title = "New Security Training Available",
                    Message = "Complete your annual security awareness training by Dec 31, 2025.",
                    Date = DateTime.UtcNow.AddDays(-3).ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                }
            };
        }
    }
}
