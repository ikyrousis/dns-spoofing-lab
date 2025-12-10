namespace LegitimateAuthService.DTOs
{    
    public class AnnouncementResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
    }
}