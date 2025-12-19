namespace EventsApi;

public class KafkaSettings
{
    public string BootstrapServers { get; set; } = string.Empty;
    public string Topic { get; set; } = "default-topic";
}
