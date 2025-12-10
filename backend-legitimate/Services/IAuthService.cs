using LegitimateAuthService.DTOs;

namespace LegitimateAuthService.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> AuthenticateAsync(LoginRequest request, string? ipAddress);
        Task<DashboardResponse> GetDashboardDataAsync(string email);
    }
}
