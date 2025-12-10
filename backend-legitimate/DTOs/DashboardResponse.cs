namespace LegitimateAuthService.DTOs
{
    public class DashboardResponse
    {
        public DashboardUserInfo User { get; set; } = new();
        public List<ResourceResponse> Resources { get; set; } = new();
        public List<AnnouncementResponse> Announcements { get; set; } = new();
        public List<ActivityLogResponse> ActivityLogs { get; set; } = new();
    }
}
