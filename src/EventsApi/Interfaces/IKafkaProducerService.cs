namespace EventsApi;

public interface IKafkaProducerService
{
    Task ProduceAsync(EventDto eventData);
}
