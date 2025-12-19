using Confluent.Kafka;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using System.Text.Json;

namespace EventsConsumer;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _config;
    private const string MeasurementName = "user_events"; // שם הטבלה ב-InfluxDB

    public Worker(ILogger<Worker> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 1. הגדרות Kafka
        var conf = new ConsumerConfig
        {
            BootstrapServers = _config["Kafka:BootstrapServers"],
            GroupId = _config["Kafka:GroupId"],
            AutoOffsetReset = AutoOffsetReset.Earliest, // אם אין היסטוריה, תתחיל מההתחלה
            EnableAutoCommit = false // אנחנו נדווח ידנית שסיימנו לעבד
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(conf).Build();
        consumer.Subscribe(_config["Kafka:Topic"]);

        // 2. הגדרות InfluxDB
        using var influxDBClient = InfluxDBClientFactory.Create(
            _config["InfluxDB:Url"], 
            _config["InfluxDB:Token"]);
            
        var writeApi = influxDBClient.GetWriteApiAsync();

        _logger.LogInformation("Worker started. Listening to Kafka...");

        // 3. הלולאה האינסופית
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // האזנה להודעה (Blocking עד שמגיעה הודעה)
                var consumeResult = consumer.Consume(stoppingToken);
                var message = consumeResult.Message.Value;

                _logger.LogInformation($"Received message: {message}");

                // המרה מ-JSON לאובייקט
                var eventData = JsonSerializer.Deserialize<EventDto>(message);

                if (eventData != null)
                {
                    // 4. בניית נקודת מידע ל-InfluxDB
                    var point = PointData
                        .Measurement(MeasurementName)
                        .Tag("type", eventData.Type)       // Tag = שדה שאפשר לפלטר לפיו מהר (אינדקס)
                        .Field("payload", eventData.Payload) // Field = המידע הגולמי
                        .Timestamp(eventData.Timestamp, WritePrecision.Ns);

                    // כתיבה ל-DB
                    await writeApi.WritePointAsync(point, _config["InfluxDB:Bucket"], _config["InfluxDB:Org"]);
                    
                    _logger.LogInformation("Written to InfluxDB!");
                }

                // דיווח לקפקא שסיימנו בהצלחה עם ההודעה הזו
                consumer.Commit(consumeResult);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing message: {ex.Message}");
            }
        }
    }
}