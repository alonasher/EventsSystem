namespace EventsApi;

public interface IInfluxQueryService
{
    Task<IReadOnlyList<EventDto>> GetEventsAsync(DateTime? from, DateTime? to, CancellationToken cancellationToken = default);
}
