namespace EventsApi;

/// <summary>
/// Produces domain events to Kafka.
/// </summary>
public interface IKafkaProducerService
{
    /// <summary>
    /// Publishes the given event to the configured Kafka topic.
    /// </summary>
    /// <param name="eventDto">The event payload to publish.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    Task ProduceAsync(EventDto eventDto, CancellationToken cancellationToken = default);
}
