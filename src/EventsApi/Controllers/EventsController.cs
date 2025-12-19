using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;

namespace EventsApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IKafkaProducerService _producer;
    private readonly IInfluxDBClient _influxClient;
    private readonly IConfiguration _config;
    private readonly ILogger<EventsController> _logger;

    public EventsController(IKafkaProducerService producer, IInfluxDBClient influxClient, IConfiguration config,ILogger<EventsController> logger)
    {
        _producer = producer;
        _influxClient = influxClient;
        _config = config;
        _logger = logger;
    }

    // בדיקת תקינות
    [HttpGet("health")]
    public IActionResult HealthCheck()
    {
        return Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
    }

    // שליחת אירוע ל-Kafka
    [HttpPost]
    public async Task<IActionResult> PublishEvent([FromBody] EventDto eventDto)
    {
        try
        {
            await _producer.ProduceAsync(eventDto);
            return Ok(new { status = "Event produced successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to produce event");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // קריאת אירועים מ-InfluxDB
    [HttpGet]
    public async Task<IActionResult> GetEvents([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        // אם לא נשלח זמן, קח שעה אחורה
        var startTime = from?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? "-1h";
        var stopTime = to?.ToString("yyyy-MM-ddTHH:mm:ssZ") ?? "now()";

        var bucket = _config["InfluxDB:Bucket"];
        var org = _config["InfluxDB:Org"];

        // שיפור: הוספנו pivot כדי שהתוצאה תהיה שטוחה וקריאה יותר ב-JSON
        var query = $@"
            from(bucket: ""{bucket}"")
            |> range(start: { (from.HasValue ? "time(v: \"" + startTime + "\")" : startTime) }, 
                     stop: { (to.HasValue ? "time(v: \"" + stopTime + "\")" : stopTime) })
            |> filter(fn: (r) => r[""_measurement""] == ""user_events"")
            |> filter(fn: (r) => r[""_field""] == ""payload"")
        ";

        try
        {
            var queryApi = _influxClient.GetQueryApi();
            var tables = await queryApi.QueryAsync(query, org);

            var results = tables.SelectMany(table => 
                table.Records.Select(record => new
                {
                    Time = record.GetTime().GetValueOrDefault().ToDateTimeUtc(),
                    Type = record.GetValueByKey("type") as string,
                    Payload = record.GetValue() as string
                })
            ).ToList();

            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to query InfluxDB");
            return StatusCode(500, new { error = ex.Message });
        }
    }
    }
}