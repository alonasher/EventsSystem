using Confluent.Kafka;
using Confluent.Kafka.Admin;
using EventsConsumer.Configuration;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace EventsConsumer;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly KafkaSettings _kafkaSettings;
    private readonly InfluxDbSettings _influxDbSettings;
    private const string MeasurementName = "user_events";

    public Worker( ILogger<Worker> logger, IOptions<KafkaSettings> kafkaOptions, IOptions<InfluxDbSettings> influxDbOptions)
    {
        _logger = logger;
        _kafkaSettings = kafkaOptions.Value;
        _influxDbSettings = influxDbOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await EnsureTopicExistsAsync();
        
        using var consumer = CreateKafkaConsumer();
        using var influxDbClient = CreateInfluxDbClient();
        var writeApi = influxDbClient.GetWriteApiAsync();

        _logger.LogInformation("Worker started. Listening to Kafka...");

        try
        {
            await ConsumeMessagesAsync(consumer, writeApi, stoppingToken);
        }
        finally
        {
            _logger.LogInformation("Worker stopped.");
        }
    }

    private async Task EnsureTopicExistsAsync()
    {
        var adminConfig = new AdminClientConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers
        };

        using var adminClient = new AdminClientBuilder(adminConfig).Build();
        
        try
        {
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
            var topicExists = metadata.Topics.Any(t => t.Topic == _kafkaSettings.Topic);

            if (!topicExists)
            {
                _logger.LogInformation("Topic '{Topic}' does not exist. Creating...", _kafkaSettings.Topic);
                
                var topicSpecification = new TopicSpecification
                {
                    Name = _kafkaSettings.Topic,
                    NumPartitions = 1,
                    ReplicationFactor = 1
                };

                await adminClient.CreateTopicsAsync(new[] { topicSpecification });
                _logger.LogInformation("Topic '{Topic}' created successfully.", _kafkaSettings.Topic);
            }
            else
            {
                _logger.LogInformation("Topic '{Topic}' already exists.", _kafkaSettings.Topic);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ensuring topic exists: {Message}", ex.Message);
            throw;
        }
    }

    private IConsumer<Ignore, string> CreateKafkaConsumer()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
            GroupId = _kafkaSettings.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(_kafkaSettings.Topic);
        return consumer;
    }

    private InfluxDBClient CreateInfluxDbClient()
    {
        return new InfluxDBClient(_influxDbSettings.Url, _influxDbSettings.Token);
    }

    private async Task ConsumeMessagesAsync( IConsumer<Ignore, string> consumer, IWriteApiAsync writeApi, CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = consumer.Consume(stoppingToken);
                await ProcessMessageAsync(consumeResult, writeApi, stoppingToken);
                consumer.Commit(consumeResult);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message: {Message}", ex.Message);
            }
        }
    }

    private async Task ProcessMessageAsync( ConsumeResult<Ignore, string> consumeResult, IWriteApiAsync writeApi, CancellationToken stoppingToken)
    {
        var message = consumeResult.Message.Value;
        _logger.LogInformation("Received message: {Message}", message);

        var eventData = JsonSerializer.Deserialize<EventDto>(message);
        if (eventData is null)
        {
            _logger.LogWarning("Failed to deserialize message");
            return;
        }

        var point = CreateDataPoint(eventData);
        await WriteToInfluxDbAsync(writeApi, point, stoppingToken);

        _logger.LogInformation("Written to InfluxDB!");
    }

    private static PointData CreateDataPoint(EventDto eventData)
    {
        return PointData
            .Measurement(MeasurementName)
            .Tag("type", eventData.Type)
            .Field("payload", eventData.Payload)
            .Timestamp(eventData.Timestamp, WritePrecision.Ns);
    }

    private async Task WriteToInfluxDbAsync( IWriteApiAsync writeApi, PointData point, CancellationToken stoppingToken)
    {
        await writeApi.WritePointAsync(
            point,
            _influxDbSettings.Bucket,
            _influxDbSettings.Org,
            stoppingToken);
    }

}