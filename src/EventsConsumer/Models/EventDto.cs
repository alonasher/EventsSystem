namespace EventsConsumer;
public record EventDto(
    string Type,
    string Payload,
    DateTime Timestamp
);