using Confluent.Kafka;
using System.Text.Json;

namespace EventsApi;

public class KafkaProducerService : IKafkaProducerService
{
    private readonly string _topic;
    private readonly IProducer<Null, string> _producer;

    public KafkaProducerService(IConfiguration config)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = config["Kafka:BootstrapServers"]
        };

        _topic = config["Kafka:Topic"] ?? "default-topic";
        _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
    }

    public async Task ProduceAsync(EventDto eventData)
    {
        var messageValue = JsonSerializer.Serialize(eventData);

        var message = new Message<Null, string> { Value = messageValue };

        // שליחה בפועל
        await _producer.ProduceAsync(_topic, message);
    }
}