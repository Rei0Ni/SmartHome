namespace SmartHome.Dto
{
    public class HealthCheckDto
    {
        public string Status { get; set; } = "Unknown";
        public List<ComponentHealthCheckDto>? Checks { get; set; }
    }
}
