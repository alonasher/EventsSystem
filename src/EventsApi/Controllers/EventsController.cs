using Microsoft.AspNetCore.Mvc;
using InfluxDB.Client;
using Microsoft.Extensions.Options;

namespace EventsApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IKafkaProducerService _producer;
        private readonly IInfluxQueryService _influxQuery;
        private readonly ILogger<EventsController> _logger;

        public EventsController( IKafkaProducerService producer, IInfluxQueryService influxQuery, ILogger<EventsController> logger)
        {
            _producer = producer;
            _influxQuery = influxQuery;
            _logger = logger;
        }

        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken cancellationToken)
        {
            try
            {
                var events = await _influxQuery.GetEventsAsync(from, to, cancellationToken);
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to query InfluxDB");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> PublishEvent([FromBody] EventDto eventDto, CancellationToken cancellationToken)
        {
            try
            {
                await _producer.ProduceAsync(eventDto, cancellationToken);
                return Ok(new { status = "Event produced successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to produce event");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}