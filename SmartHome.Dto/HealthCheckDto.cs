namespace SmartHome.Dto
{
    public class HealthCheckDto
    {
        public required string Status { get; set; }
        public List<ComponentHealthCheckDto>? Checks { get; set; }
    }
}
