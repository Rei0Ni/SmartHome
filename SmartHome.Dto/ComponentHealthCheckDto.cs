namespace SmartHome.Dto
{
    public class ComponentHealthCheckDto
    {
        public required string Component { get; set; }
        public required string Status { get; set; }
        public Dictionary<string, object>? Details { get; set; }
    }
}
