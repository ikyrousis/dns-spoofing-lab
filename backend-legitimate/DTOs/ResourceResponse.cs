namespace LegitimateAuthService.DTOs
{
    public class ResourceResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
    }
}