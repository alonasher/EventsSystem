using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace EventsApi;

public class KafkaProducerService : IKafkaProducerService
{
    private readonly string _topic;
    private readonly IProducer<Null, string> _producer;
    private readonly ILogger<KafkaProducerService> _logger;

    public KafkaProducerService(IOptions<KafkaSettings> options, ILogger<KafkaProducerService> logger)
    {
        var kafka = options.Value;
        if (string.IsNullOrWhiteSpace(kafka.BootstrapServers))
        {
            throw new ArgumentException("Kafka BootstrapServers must be configured.");
        }

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = kafka.BootstrapServers
        };

        _topic = kafka.Topic ?? "default-topic";
        _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        _logger = logger;
    }

    public async Task ProduceAsync(EventDto eventDto, CancellationToken cancellationToken = default)
    {
        if (eventDto is null)
        {
            throw new ArgumentNullException(nameof(eventDto));
        }

        var messageValue = JsonSerializer.Serialize(eventDto);

        var message = new Message<Null, string> { Value = messageValue };

        try
        {
            var result = await _producer.ProduceAsync(_topic, message, cancellationToken);
            _logger.LogInformation("Produced event to {Topic} [partition {Partition}, offset {Offset}]", result.Topic, result.Partition, result.Offset);
        }
        catch (ProduceException<Null, string> ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Failed to produce event to {Topic}", _topic);
            throw;
        }
    }
}