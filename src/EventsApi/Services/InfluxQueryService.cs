using InfluxDB.Client;
using Microsoft.Extensions.Options;

namespace EventsApi;

public class InfluxQueryService : IInfluxQueryService
{
    private readonly IInfluxDBClient _client;
    private readonly InfluxDbSettings _settings;

    public InfluxQueryService(IInfluxDBClient client, IOptions<InfluxDbSettings> settings)
    {
        _client = client;
        _settings = settings.Value;
    }

    public async Task<IReadOnlyList<EventDto>> GetEventsAsync(DateTime? from, DateTime? to, CancellationToken cancellationToken = default)
    {
        if (from.HasValue && to.HasValue && from > to)
        {
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
                    |> filter(fn: (r) => r[""_measurement""] == ""user_events"")
                    |> filter(fn: (r) => r[""_field""] == ""payload"")";

        var queryApi = _client.GetQueryApi();
        var tables = await queryApi.QueryAsync(query, _settings.Org, cancellationToken);

        var results = tables
            .SelectMany(t => t.Records)
            .Select(r => new EventDto(
                Type: r.GetValueByKey("type") as string ?? "unknown",
                Payload: r.GetValue() as string ?? "",
                Timestamp: r.GetTime().GetValueOrDefault().ToDateTimeUtc()
            ))
            .ToList();

        return results;
    }
}
