namespace SmartHome.Dto.ESPConfig
{
    public class MqttConfigDto
    {
        public string server { get; set; } = "192.168.1.9";
        public int publishInterval { get; set; } = 1883;
    }
}
