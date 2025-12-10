namespace LegitimateAuthService.DTOs
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserResponse? User { get; set; }
    }
}
