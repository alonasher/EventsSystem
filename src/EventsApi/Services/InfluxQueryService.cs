using InfluxDB.Client;
using Microsoft.Extensions.Options;

namespace EventsApi;

public class InfluxQueryService : IInfluxQueryService
{
    private const string MeasurementFilter = "user_events";
    private const string FieldFilter = "payload";
    private const string TypeKey = "type";
    private const string DefaultType = "unknown";
    private const string DefaultPayload = "";

    private readonly IInfluxDBClient _client;
    private readonly InfluxDbSettings _settings;
    private readonly ILogger<InfluxQueryService> _logger;

    public InfluxQueryService(IInfluxDBClient client, IOptions<InfluxDbSettings> settings, ILogger<InfluxQueryService> logger)
    {
        _client = client;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyList<EventDto>> GetEventsAsync(DateTime? from, DateTime? to, CancellationToken cancellationToken = default)
    {
        if (from.HasValue && to.HasValue && from > to)
        {
            _logger.LogWarning("Invalid date range: from ({From}) > to ({To})", from, to);
            throw new ArgumentException("'from' must be earlier than or equal to 'to'.");
        }

        var startExpr = from.HasValue
            ? $"time(v: \"{from.Value.ToString("yyyy-MM-ddTHH:mm:ssZ")}\")"
            : "-1h";

        var stopExpr = to.HasValue
            ? $"time(v: \"{to.Value.ToString("yyyy-MM-ddTHH:mm:ssZ")}\")"
            : "now()";

        var query = $@"from(bucket: ""{_settings.Bucket}"")
            |> range(start: {startExpr}, stop: {stopExpr})
            |> filter(fn: (r) => r[""_measurement""] == ""{MeasurementFilter}"")
            |> filter(fn: (r) => r[""_field""] == ""{FieldFilter}"")";

        try
        {
            _logger.LogDebug("Executing InfluxDB query for range [{From}, {To}]", from ?? DateTime.MinValue, to ?? DateTime.MaxValue);

            var queryApi = _client.GetQueryApi();
            var tables = await queryApi.QueryAsync(query, _settings.Org, cancellationToken);

            var results = tables
                .SelectMany(t => t.Records)
                .Select(r => new EventDto(
                    Type: r.GetValueByKey(TypeKey) as string ?? DefaultType,
                    Payload: r.GetValue() as string ?? DefaultPayload,
                    Timestamp: r.GetTime().GetValueOrDefault().ToDateTimeUtc()
                ))
                .ToList();

            _logger.LogDebug("Retrieved {RecordCount} events from InfluxDB", results.Count);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to query InfluxDB for range [{From}, {To}]", from, to);
            throw;
        }
    }
}
