namespace LegitimateAuthService.DTOs
{
    public class ActivityLogResponse
    {
        public string Action { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string Timestamp { get; set; } = string.Empty;
    }
}